using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using VideoDownloader.Models;
using VideoDownloader.Strategies;

namespace VideoDownloader.Services
{
    public class YtDlpService
    {
        private readonly List<IPlatformStrategy> _strategies;
        private Process _currentProcess;
        private bool _isCancelled;
        private bool _isPaused;
        private string _lastErrorLine = string.Empty;
        private static readonly Regex ArgumentTokenizer = new Regex("\"[^\"]*\"|\\S+", RegexOptions.Compiled);

        public event Action<string> OutputReceived;
        public event Action<double, string> ProgressChanged;
        public event Action<bool, string> DownloadCompleted;

        public YtDlpService()
        {
            _strategies = new List<IPlatformStrategy>
            {
                new KickStrategy(),
                new TwitchStrategy(),
                new DefaultStrategy()
            };
        }

        public async Task<VideoMetadata> GetVideoMetadataAsync(string url, bool useStandalone, string standalonePath, CancellationToken token)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var strategy = _strategies.FirstOrDefault(s => s.CanHandle(url)) ?? new DefaultStrategy();
                    string extraArgs = strategy.GetExtraArguments(url);

                    bool runStandalone = useStandalone && !string.IsNullOrEmpty(standalonePath);
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = runStandalone ? standalonePath : "python",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8
                    };

                    if (!runStandalone)
                    {
                        startInfo.ArgumentList.Add("-m");
                        startInfo.ArgumentList.Add("yt_dlp");
                    }

                    AddArgumentsFromString(startInfo, extraArgs);
                    startInfo.ArgumentList.Add("--no-download");
                    startInfo.ArgumentList.Add("--print-json");
                    startInfo.ArgumentList.Add(url);

                    using var process = new Process { StartInfo = startInfo };
                    process.Start();

                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(15000);

                    if (token.IsCancellationRequested) return null;

                    if (!string.IsNullOrEmpty(output))
                    {
                        using var doc = JsonDocument.Parse(output);
                        var root = doc.RootElement;

                        string thumbnailUrl = "";
                        if (root.TryGetProperty("thumbnails", out var thumbnails) && thumbnails.ValueKind == JsonValueKind.Array)
                        {
                            var thumbList = thumbnails.EnumerateArray().ToList();
                            var jpgThumbs = thumbList.Where(t => t.TryGetProperty("url", out var u) && (u.GetString()?.Contains(".jpg") == true)).ToList();
                            if (jpgThumbs.Any()) thumbnailUrl = jpgThumbs.Last().GetProperty("url").GetString() ?? "";
                            else if (thumbList.Any()) thumbnailUrl = thumbList.Last().GetProperty("url").GetString() ?? "";
                        }
                        
                        if (string.IsNullOrEmpty(thumbnailUrl) && root.TryGetProperty("thumbnail", out var thumb))
                            thumbnailUrl = thumb.GetString() ?? "";

                        return new VideoMetadata
                        {
                            Title = root.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                            Channel = root.TryGetProperty("uploader", out var uploader) ? uploader.GetString() ?? "" :
                                     (root.TryGetProperty("channel", out var channel) ? channel.GetString() ?? "" : ""),
                            Duration = root.TryGetProperty("duration", out var duration) && duration.ValueKind == JsonValueKind.Number ? duration.GetInt32() : 0,
                            ThumbnailUrl = thumbnailUrl
                        };
                    }
                }
                catch { }
                return null;
            }, token);
        }

        public async Task DownloadAsync(string url, string outputPath, string qualityArg, bool downloadSubs, bool useStandalone, string standalonePath, string ffmpegPath, bool ffmpegAvailable)
        {
            _isCancelled = false;
            _isPaused = false;
            _lastErrorLine = string.Empty;

            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(url)) ?? new DefaultStrategy();
            string extraArgs = strategy.GetExtraArguments(url);

            bool runStandalone = useStandalone && !string.IsNullOrEmpty(standalonePath);
            var startInfo = new ProcessStartInfo
            {
                FileName = runStandalone ? standalonePath : "python",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            if (!runStandalone)
            {
                startInfo.ArgumentList.Add("-m");
                startInfo.ArgumentList.Add("yt_dlp");
            }

            AddArgumentsFromString(startInfo, extraArgs);

            if (!string.IsNullOrWhiteSpace(ffmpegPath))
            {
                startInfo.ArgumentList.Add("--ffmpeg-location");
                startInfo.ArgumentList.Add(ffmpegPath);
            }

            startInfo.ArgumentList.Add("--continue");
            startInfo.ArgumentList.Add("--no-playlist");

            AddArgumentsFromString(startInfo, qualityArg);

            if (downloadSubs)
            {
                startInfo.ArgumentList.Add("--embed-subs");
                startInfo.ArgumentList.Add("--write-auto-sub");
            }

            if (ffmpegAvailable)
            {
                startInfo.ArgumentList.Add("--embed-thumbnail");
                startInfo.ArgumentList.Add("--merge-output-format");
                startInfo.ArgumentList.Add("mp4");
            }

            startInfo.ArgumentList.Add("-o");
            startInfo.ArgumentList.Add(Path.Combine(outputPath, "%(title)s.%(ext)s"));
            startInfo.ArgumentList.Add(url);

            _currentProcess = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

            _currentProcess.OutputDataReceived += (s, e) => HandleOutput(e.Data);
            _currentProcess.ErrorDataReceived += (s, e) => HandleOutput(e.Data);

            await Task.Run(() =>
            {
                try
                {
                    _currentProcess.Start();
                    _currentProcess.BeginOutputReadLine();
                    _currentProcess.BeginErrorReadLine();
                    _currentProcess.WaitForExit();
                    
                    if (!_isCancelled)
                    {
                        bool success = _currentProcess.ExitCode == 0;
                        string message = success
                            ? "Success"
                            : (!string.IsNullOrWhiteSpace(_lastErrorLine) ? _lastErrorLine : "Process exited with error");
                        DownloadCompleted?.Invoke(success, message);
                    }
                }
                catch (Exception ex)
                {
                    DownloadCompleted?.Invoke(false, ex.Message);
                }
                finally
                {
                    _currentProcess?.Dispose();
                    _currentProcess = null;
                }
            });
        }

        private static void AddArgumentsFromString(ProcessStartInfo startInfo, string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments)) return;

            foreach (Match match in ArgumentTokenizer.Matches(arguments))
            {
                var token = match.Value;
                if (token.Length >= 2 && token.StartsWith("\"") && token.EndsWith("\""))
                {
                    token = token.Substring(1, token.Length - 2);
                }

                if (!string.IsNullOrWhiteSpace(token))
                {
                    startInfo.ArgumentList.Add(token);
                }
            }
        }

        private void HandleOutput(string data)
        {
            if (string.IsNullOrEmpty(data) || _isCancelled) return;

            OutputReceived?.Invoke(data);

            if (data.Contains("ERROR:", StringComparison.OrdinalIgnoreCase))
            {
                _lastErrorLine = data.Trim();
            }

            // Progress parsing logic (simplified version of what's in MainForm)
            if (data.Contains("[download]") && data.Contains("%"))
            {
                try
                {
                    var parts = data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (part.Contains("%"))
                        {
                            if (double.TryParse(part.TrimEnd('%').Replace('.', ','), out double progress))
                            {
                                ProgressChanged?.Invoke(progress, "Downloading");
                                break;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        public void Cancel()
        {
            _isCancelled = true;
            KillProcessTree(_currentProcess?.Id ?? 0);
        }

        public void PauseResume()
        {
            if (_currentProcess == null) return;

            if (_isPaused)
            {
                ResumeProcess(_currentProcess.Id);
                _isPaused = false;
            }
            else
            {
                PauseProcess(_currentProcess.Id);
                _isPaused = true;
            }
        }

        // Windows specific process control (should be moved to a native helper or kept here for simplicity)
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hHandle);

        private void PauseProcess(int pid)
        {
            try {
                var process = Process.GetProcessById(pid);
                foreach (ProcessThread thread in process.Threads)
                {
                    var pOpenThread = OpenThread(0x0002, false, (uint)thread.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        SuspendThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }
                }
            } catch { }
        }

        private void ResumeProcess(int pid)
        {
            try {
                var process = Process.GetProcessById(pid);
                foreach (ProcessThread thread in process.Threads)
                {
                    var pOpenThread = OpenThread(0x0002, false, (uint)thread.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        ResumeThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }
                }
            } catch { }
        }

        private void KillProcessTree(int pid)
        {
            if (pid == 0) return;
            try {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = $"/T /F /PID {pid}",
                    CreateNoWindow = true,
                    UseShellExecute = false
                })?.WaitForExit();
            } catch { }
        }
    }
}
