using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace VideoDownloader
{
    public enum AppLanguage
    {
        Turkish,
        English
    }

    public enum AppTheme
    {
        Light,
        Dark
    }

    public partial class MainForm : Form
    {
        private DateTime downloadStartTime;
        private long totalBytes = 0;
        private long downloadedBytes = 0;
        private AppLanguage currentLanguage = AppLanguage.Turkish;
        private AppTheme currentTheme = AppTheme.Light;
        private Dictionary<string, Dictionary<AppLanguage, string>> translations;
        private MenuStrip mainMenuStrip;

        public MainForm()
        {
            InitializeComponent();
            InitializeTranslations();
            LoadSettings();
            InitializeAboutMenu();
            InitializeDefaultValues();
            ApplyTheme();
            ApplyLanguage();
        }

        private void InitializeDefaultValues()
        {
            platformComboBox.SelectedIndex = 0;
            qualityComboBox.SelectedIndex = 3;
            pathTextBox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), GetText("FolderVideos"));
        }

        private void ChangeLanguage(AppLanguage newLanguage)
        {
            currentLanguage = newLanguage;
            SaveSettings();
            ApplyLanguage();
        }

        private void ChangeTheme(AppTheme newTheme)
        {
            currentTheme = newTheme;
            SaveSettings();
            ApplyTheme();
        }

        private void ApplyLanguage()
        {
            this.Text = GetText("FormTitle");
            urlLabel.Text = GetText("VideoURL");
            platformLabel.Text = GetText("Platform");
            qualityLabel.Text = GetText("Quality");
            pathLabel.Text = GetText("DownloadPath");
            urlTextBox.PlaceholderText = GetText("URLPlaceholder");
            browseButton.Text = GetText("Browse");
            downloadButton.Text = GetText("DownloadVideo");
            updateButton.Text = GetText("SystemCheck");
            formatButton.Text = GetText("FormatList");
            subtitleCheckBox.Text = GetText("DownloadSubtitles");
            progressGroupBox.Text = GetText("DownloadStatus");
            UpdateComboBoxItems();
            RefreshMenuStrip();
            if (statusLabel.Text.Contains("Hazır") || statusLabel.Text.Contains("Ready"))
            {
                statusLabel.Text = GetText("Ready") + " - kayapater tarafından geliştirildi";
            }
        }

        private void UpdateComboBoxItems()
        {
            var platformSelection = platformComboBox.SelectedIndex;
            platformComboBox.Items.Clear();
            platformComboBox.Items.AddRange(new object[] 
            { 
                GetText("Auto"), 
                "YouTube", 
                "Twitter", 
                "Instagram" 
            });
            platformComboBox.SelectedIndex = platformSelection;

            var qualitySelection = qualityComboBox.SelectedIndex;
            qualityComboBox.Items.Clear();
            qualityComboBox.Items.AddRange(new object[] 
            { 
                GetText("BestQuality"), 
                "2160p", 
                "1440p", 
                "1080p", 
                "720p", 
                "480p", 
                "360p", 
                GetText("AudioOnly") 
            });
            qualityComboBox.SelectedIndex = qualitySelection;
        }

        private void RefreshMenuStrip()
        {
            this.Controls.Remove(mainMenuStrip);
            InitializeAboutMenu();
        }

        private void ApplyTheme()
        {
            Color backgroundColor, foregroundColor, buttonColor, inputBackColor, inputForeColor;

            if (currentTheme == AppTheme.Dark)
            {
                backgroundColor = Color.FromArgb(32, 32, 32);
                foregroundColor = Color.FromArgb(220, 220, 220);
                buttonColor = Color.FromArgb(45, 45, 45);
                inputBackColor = Color.FromArgb(45, 45, 45);
                inputForeColor = Color.FromArgb(220, 220, 220);
            }
            else
            {
                backgroundColor = Color.FromArgb(248, 249, 250);
                foregroundColor = Color.FromArgb(45, 45, 45);
                buttonColor = Color.White;
                inputBackColor = Color.White;
                inputForeColor = Color.FromArgb(45, 45, 45);
            }

            this.BackColor = backgroundColor;

            foreach (Control control in this.Controls)
            {
                if (control is Label label)
                {
                    label.ForeColor = foregroundColor;
                }
                else if (control is TextBox textBox)
                {
                    textBox.BackColor = inputBackColor;
                    textBox.ForeColor = inputForeColor;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.BackColor = inputBackColor;
                    comboBox.ForeColor = inputForeColor;
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.BackColor = backgroundColor;
                    checkBox.ForeColor = foregroundColor;
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.BackColor = backgroundColor;
                    groupBox.ForeColor = foregroundColor;
                    
                    foreach (Control innerControl in groupBox.Controls)
                    {
                        if (innerControl is Label innerLabel)
                        {
                            innerLabel.ForeColor = foregroundColor;
                        }
                    }
                }
                else if (control is RichTextBox richTextBox)
                {
                    if (currentTheme == AppTheme.Dark)
                    {
                        richTextBox.BackColor = Color.FromArgb(20, 20, 20);
                        richTextBox.ForeColor = Color.FromArgb(40, 167, 69);
                    }
                    else
                    {
                        richTextBox.BackColor = Color.FromArgb(33, 37, 41);
                        richTextBox.ForeColor = Color.FromArgb(40, 167, 69);
                    }
                }
                else if (control is MenuStrip menuStrip)
                {
                    menuStrip.BackColor = backgroundColor;
                    menuStrip.ForeColor = foregroundColor;
                    
                    foreach (ToolStripItem item in menuStrip.Items)
                    {
                        item.BackColor = backgroundColor;
                        item.ForeColor = foregroundColor;
                    }
                }
            }
        }

        private void InitializeTranslations()
        {
            translations = new Dictionary<string, Dictionary<AppLanguage, string>>
            {
                ["FormTitle"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Video İndirici - YouTube, Twitter, Instagram",
                    [AppLanguage.English] = "Video Downloader - YouTube, Twitter, Instagram"
                },
                ["VideoURL"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Video URL:",
                    [AppLanguage.English] = "Video URL:"
                },
                ["URLPlaceholder"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "YouTube, Twitter veya Instagram video linkini buraya yapıştırın...",
                    [AppLanguage.English] = "Paste YouTube, Twitter or Instagram video link here..."
                },
                ["Platform"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Platform:",
                    [AppLanguage.English] = "Platform:"
                },
                ["Auto"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Otomatik",
                    [AppLanguage.English] = "Automatic"
                },
                ["Quality"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Kalite:",
                    [AppLanguage.English] = "Quality:"
                },
                ["BestQuality"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "En İyi",
                    [AppLanguage.English] = "Best"
                },
                ["AudioOnly"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Sadece Ses",
                    [AppLanguage.English] = "Audio Only"
                },
                ["DownloadSubtitles"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Altyazı İndir",
                    [AppLanguage.English] = "Download Subtitles"
                },
                ["DownloadPath"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "İndirme Yolu:",
                    [AppLanguage.English] = "Download Path:"
                },
                ["Browse"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "📁 Gözat",
                    [AppLanguage.English] = "📁 Browse"
                },
                ["DownloadVideo"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "🎬 Video İndir",
                    [AppLanguage.English] = "🎬 Download Video"
                },
                ["SystemCheck"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "🔧 Sistem Kontrolü",
                    [AppLanguage.English] = "🔧 System Check"
                },
                ["FormatList"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "📋 Format Listesi",
                    [AppLanguage.English] = "📋 Format List"
                },
                ["DownloadStatus"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "📊 İndirme Durumu",
                    [AppLanguage.English] = "📊 Download Status"
                },
                ["Ready"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Hazır",
                    [AppLanguage.English] = "Ready"
                },
                ["Preparing"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Hazırlanıyor...",
                    [AppLanguage.English] = "Preparing..."
                },
                ["Settings"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Ayarlar",
                    [AppLanguage.English] = "Settings"
                },
                ["Language"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Dil",
                    [AppLanguage.English] = "Language"
                },
                ["Turkish"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Türkçe",
                    [AppLanguage.English] = "Turkish"
                },
                ["English"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "İngilizce",
                    [AppLanguage.English] = "English"
                },
                ["Theme"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Tema",
                    [AppLanguage.English] = "Theme"
                },
                ["LightTheme"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Açık Tema",
                    [AppLanguage.English] = "Light Theme"
                },
                ["DarkTheme"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Koyu Tema",
                    [AppLanguage.English] = "Dark Theme"
                },
                ["Help"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Yardım",
                    [AppLanguage.English] = "Help"
                },
                ["About"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Hakkında",
                    [AppLanguage.English] = "About"
                },
                ["AboutTitle"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Video İndirici Hakkında",
                    [AppLanguage.English] = "About Video Downloader"
                },
                ["AppDescription"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Video İndirici v1.3\n\nYouTube, Twitter ve Instagram'dan video indirme aracı",
                    [AppLanguage.English] = "Video Downloader v1.3\n\nDownload videos from YouTube, Twitter and Instagram"
                },
                ["Developer"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Geliştirici:",
                    [AppLanguage.English] = "Developer:"
                },
                ["OK"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Tamam",
                    [AppLanguage.English] = "OK"
                },
                ["FolderVideos"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "İndirilen Videolar",
                    [AppLanguage.English] = "Downloaded Videos"
                }
            };
        }

        private void LoadSettings()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\VideoDownloader"))
                {
                    var langValue = key?.GetValue("Language")?.ToString();
                    if (Enum.TryParse<AppLanguage>(langValue, out var language))
                    {
                        currentLanguage = language;
                    }

                    var themeValue = key?.GetValue("Theme")?.ToString();
                    if (Enum.TryParse<AppTheme>(themeValue, out var theme))
                    {
                        currentTheme = theme;
                    }
                }
            }
            catch
            {
            }
        }

        private void SaveSettings()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\VideoDownloader"))
                {
                    key?.SetValue("Language", currentLanguage.ToString());
                    key?.SetValue("Theme", currentTheme.ToString());
                }
            }
            catch
            {
            }
        }

        private string GetText(string key)
        {
            if (translations.ContainsKey(key) && translations[key].ContainsKey(currentLanguage))
            {
                return translations[key][currentLanguage];
            }
            return key;
        }

        private void InitializeAboutMenu()
        {
            mainMenuStrip = new MenuStrip();
            
            var settingsMenu = new ToolStripMenuItem(GetText("Settings"));
            
            var languageMenu = new ToolStripMenuItem(GetText("Language"));
            var turkishMenuItem = new ToolStripMenuItem(GetText("Turkish"));
            var englishMenuItem = new ToolStripMenuItem(GetText("English"));
            
            turkishMenuItem.Click += (s, e) => ChangeLanguage(AppLanguage.Turkish);
            englishMenuItem.Click += (s, e) => ChangeLanguage(AppLanguage.English);
            
            languageMenu.DropDownItems.AddRange(new ToolStripItem[] { turkishMenuItem, englishMenuItem });
            
            var themeMenu = new ToolStripMenuItem(GetText("Theme"));
            var lightThemeMenuItem = new ToolStripMenuItem(GetText("LightTheme"));
            var darkThemeMenuItem = new ToolStripMenuItem(GetText("DarkTheme"));
            
            lightThemeMenuItem.Click += (s, e) => ChangeTheme(AppTheme.Light);
            darkThemeMenuItem.Click += (s, e) => ChangeTheme(AppTheme.Dark);
            
            themeMenu.DropDownItems.AddRange(new ToolStripItem[] { lightThemeMenuItem, darkThemeMenuItem });
            
            settingsMenu.DropDownItems.AddRange(new ToolStripItem[] { languageMenu, themeMenu });
            
            var helpMenu = new ToolStripMenuItem(GetText("Help"));
            var aboutMenuItem = new ToolStripMenuItem(GetText("About"));
            
            aboutMenuItem.Click += AboutMenuItem_Click;
            helpMenu.DropDownItems.Add(aboutMenuItem);
            
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { settingsMenu, helpMenu });

            this.MainMenuStrip = mainMenuStrip;
            this.Controls.Add(mainMenuStrip);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            Color backgroundColor, foregroundColor;
            if (currentTheme == AppTheme.Dark)
            {
                backgroundColor = Color.FromArgb(32, 32, 32);
                foregroundColor = Color.FromArgb(220, 220, 220);
            }
            else
            {
                backgroundColor = Color.FromArgb(248, 249, 250);
                foregroundColor = Color.FromArgb(45, 45, 45);
            }

            var aboutForm = new Form
            {
                Text = GetText("AboutTitle"),
                Size = new Size(520, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = backgroundColor
            };

            var textLabel = new Label
            {
                Text = GetText("AppDescription"),
                Location = new Point(20, 20),
                Size = new Size(400, 80),
                Font = new Font("Segoe UI", 10),
                ForeColor = foregroundColor
            };

            var developerLabel = new Label
            {
                Text = GetText("Developer"),
                Location = new Point(20, 100),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = foregroundColor
            };

            var kayapaterLink = new LinkLabel
            {
                Text = "kayapater",
                Location = new Point(105, 100),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = backgroundColor
            };
            kayapaterLink.LinkClicked += (s, args) =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo("https:
                }
                catch (Exception ex)
                {
                    var errorMsg = currentLanguage == AppLanguage.Turkish ? 
                        $"Link açılamadı: {ex.Message}" : 
                        $"Could not open link: {ex.Message}";
                    var errorTitle = currentLanguage == AppLanguage.Turkish ? "Hata" : "Error";
                    MessageBox.Show(errorMsg, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            var featuresText = currentLanguage == AppLanguage.Turkish ? 
                @"Bu uygulama ile:
• YouTube videolarını indirebilirsiniz
• Twitter videolarını indirebilirsiniz  
• Instagram videolarını indirebilirsiniz
• Farklı kalite seçenekleri kullanabilirsiniz
• Altyazı indirme desteği mevcuttur
• İngilizce ve Türkçe dil desteği
• Açık ve koyu tema seçenekleri

Teknolojiler:
• .NET 8.0 Windows Forms
• yt-dlp (Python video indirme modülü)
• FFmpeg (video/ses işleme ve dönüştürme)
• Python 3.x (backend motor)
• Newtonsoft.Json (JSON veri işleme)
• Windows API (sistem entegrasyonu)
• C# async/await (asenkron işlemler)
• Process API (harici program çalıştırma)" :
                @"With this application you can:
• Download videos from YouTube
• Download videos from Twitter  
• Download videos from Instagram
• Use different quality options
• Download subtitles support
• English and Turkish language support
• Light and dark theme options

Technologies:
• .NET 8.0 Windows Forms
• yt-dlp (Python video download module)
• FFmpeg (video/audio processing)
• Python 3.x (backend engine)
• Newtonsoft.Json (JSON data processing)
• Windows API (system integration)
• C# async/await (asynchronous operations)
• Process API (external program execution)";

            var featuresLabel = new Label
            {
                Text = featuresText,
                Location = new Point(20, 130),
                Size = new Size(460, 240),
                Font = new Font("Segoe UI", 9),
                ForeColor = foregroundColor
            };

            var okButton = new Button
            {
                Text = GetText("OK"),
                Location = new Point(410, 390),
                Size = new Size(80, 35),
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            aboutForm.Controls.AddRange(new Control[] { textLabel, developerLabel, kayapaterLink, featuresLabel, okButton });
            aboutForm.AcceptButton = okButton;
            aboutForm.ShowDialog(this);
        }

        private void ShowProgress(bool show)
        {
            progressGroupBox.Visible = show;
            if (show)
            {
                progressBar.Value = 0;
                progressPercentageLabel.Text = "0%";
                progressStatusLabel.Text = "Hazırlanıyor...";
                currentFileLabel.Text = "";
                speedLabel.Text = "";
                timeRemainingLabel.Text = "";
            }
        }

        private void UpdateProgress(int percentage, string status = "", string currentFile = "", string speed = "", string timeRemaining = "")
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateProgress(percentage, status, currentFile, speed, timeRemaining));
                return;
            }

            if (percentage >= 0 && percentage <= 100)
            {
                progressBar.Value = percentage;
                progressPercentageLabel.Text = $"{percentage}%";
            }
            
            if (!string.IsNullOrEmpty(status))
                progressStatusLabel.Text = status;
            
            if (!string.IsNullOrEmpty(currentFile))
                currentFileLabel.Text = $"📁 {currentFile}";
            
            if (!string.IsNullOrEmpty(speed))
                speedLabel.Text = $"⚡ {speed}";
            
            if (!string.IsNullOrEmpty(timeRemaining))
                timeRemainingLabel.Text = $"⏱️ {timeRemaining}";
        }

        private void UpdateProgressText(string status = "", string currentFile = "", string speed = "", string timeRemaining = "")
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateProgressText(status, currentFile, speed, timeRemaining));
                return;
            }
            
            if (!string.IsNullOrEmpty(status))
                progressStatusLabel.Text = status;
            
            if (!string.IsNullOrEmpty(currentFile))
                currentFileLabel.Text = $"📁 {currentFile}";
            
            if (!string.IsNullOrEmpty(speed))
                speedLabel.Text = $"⚡ {speed}";
            
            if (!string.IsNullOrEmpty(timeRemaining))
                timeRemainingLabel.Text = $"⏱️ {timeRemaining}";
        }

        private void LogMessage(string message, System.Drawing.Color color)
        {
            if (logTextBox.InvokeRequired)
            {
                logTextBox.Invoke(() => LogMessage(message, color));
                return;
            }

            logTextBox.SelectionStart = logTextBox.TextLength;
            logTextBox.SelectionLength = 0;
            logTextBox.SelectionColor = color;
            logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            logTextBox.SelectionColor = logTextBox.ForeColor;
            logTextBox.ScrollToCaret();
        }

        private void ShowCriticalError(string message, string title = "Hata")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowWarning(string message, string title = "Uyarı")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowSuccess(string message, string title = "Başarılı")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "İndirilen videoların kaydedileceği klasörü seçin";
            folderDialog.SelectedPath = pathTextBox.Text;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private async void FormatButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlTextBox.Text))
            {
                ShowWarning("Lütfen önce bir video URL'si girin!");
                return;
            }

            var formatButton = sender as Button;
            formatButton.Enabled = false;
            ShowProgress(true);
            UpdateProgress(0, "Mevcut formatlar kontrol ediliyor...");

            try
            {
                statusLabel.Text = "Format listesi kontrol ediliyor...";

                var processInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"-m yt_dlp --list-formats \"{urlTextBox.Text.Trim()}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };

                string output = "";
                process.OutputDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        output += args.Data + "\n";
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode == 0)
                {
                    UpdateProgress(100, "Format listesi hazır");
                    var formatForm = new Form
                    {
                        Text = "Mevcut Video Formatları",
                        Size = new Size(800, 600),
                        StartPosition = FormStartPosition.CenterParent
                    };
                    
                    var textBox = new TextBox
                    {
                        Multiline = true,
                        ScrollBars = ScrollBars.Both,
                        Dock = DockStyle.Fill,
                        Text = output,
                        ReadOnly = true,
                        Font = new Font("Consolas", 9)
                    };
                    
                    formatForm.Controls.Add(textBox);
                    formatForm.ShowDialog(this);
                }
                else
                {
                    ShowCriticalError("Format listesi alınamadı. URL'yi kontrol edin.");
                }

                statusLabel.Text = "Hazır";
            }
            catch (Exception ex)
            {
                ShowCriticalError($"Format listesi hatası: {ex.Message}");
            }
            finally
            {
                formatButton.Enabled = true;
                ShowProgress(false);
                statusLabel.Text = "Hazır";
            }
        }

        private async void UpdateButton_Click(object sender, EventArgs e)
        {
            updateButton.Enabled = false;
            downloadButton.Enabled = false;
            ShowProgress(true);
            UpdateProgress(0, "Sistem gereksinimleri kontrol ediliyor...");

            try
            {
                bool allDependenciesOk = true;
                var missingComponents = new List<string>();

                UpdateProgress(10, "Python kontrolü yapılıyor...");
                LogMessage("Python kurulum kontrolü başlatıldı", Color.LimeGreen);
                
                if (!await CheckPythonInstalled())
                {
                    missingComponents.Add("Python");
                    allDependenciesOk = false;
                    LogMessage("❌ Python bulunamadı", Color.Red);
                    
                    if (MessageBox.Show("Python bulunamadı! Otomatik olarak kurulmasını ister misiniz?\n\n" +
                        "Bu işlem birkaç dakika sürebilir ve internet bağlantısı gerekir.", 
                        "Python Kurulumu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        UpdateProgress(20, "Python kuruluyor...");
                        if (await InstallPython())
                        {
                            LogMessage("✅ Python başarıyla kuruldu", Color.LimeGreen);
                            missingComponents.Remove("Python");
                        }
                        else
                        {
                            LogMessage("❌ Python kurulumu başarısız", Color.Red);
                        }
                    }
                }
                else
                {
                    LogMessage("✅ Python kurulu", Color.LimeGreen);
                }

                UpdateProgress(40, "yt-dlp kontrolü yapılıyor...");
                LogMessage("yt-dlp kurulum/güncelleme kontrolü başlatıldı", Color.LimeGreen);
                
                if (!await CheckYtDlpInstalled())
                {
                    missingComponents.Add("yt-dlp");
                    LogMessage("❌ yt-dlp bulunamadı", Color.Red);
                }

                UpdateProgress(50, "yt-dlp kuruluyor/güncelleniyor...");
                if (await InstallYtDlp())
                {
                    LogMessage("✅ yt-dlp başarıyla kuruldu/güncellendi", Color.LimeGreen);
                    if (missingComponents.Contains("yt-dlp"))
                        missingComponents.Remove("yt-dlp");
                }
                else
                {
                    LogMessage("❌ yt-dlp kurulumu/güncellemesi başarısız", Color.Red);
                    allDependenciesOk = false;
                }

                UpdateProgress(70, "FFmpeg kontrolü yapılıyor...");
                LogMessage("FFmpeg kurulum kontrolü başlatıldı", Color.LimeGreen);
                
                if (!await CheckFFmpegInstalled())
                {
                    missingComponents.Add("FFmpeg");
                    LogMessage("⚠️ FFmpeg bulunamadı (opsiyonel)", Color.Orange);
                    
                    if (MessageBox.Show("FFmpeg bulunamadı! Bu ses/video birleştirme için gereklidir.\n\n" +
                        "Otomatik olarak kurulmasını ister misiniz?\n\n" +
                        "Not: Bu işlem winget gerektirir ve birkaç dakika sürebilir.", 
                        "FFmpeg Kurulumu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        UpdateProgress(80, "FFmpeg kuruluyor...");
                        if (await InstallFFmpeg())
                        {
                            LogMessage("✅ FFmpeg başarıyla kuruldu", Color.LimeGreen);
                            missingComponents.Remove("FFmpeg");
                        }
                        else
                        {
                            LogMessage("⚠️ FFmpeg kurulumu başarısız - manuel kurulum gerekebilir", Color.Orange);
                        }
                    }
                }
                else
                {
                    LogMessage("✅ FFmpeg kurulu", Color.LimeGreen);
                }

                UpdateProgress(100, "Kontrol tamamlandı!");

                if (missingComponents.Count == 0)
                {
                    statusLabel.Text = "Tüm gereksinimler karşılandı!";
                    ShowSuccess("🎉 Sistem Gereksinim Kontrolü Tamamlandı!\n\n" +
                        "✅ Python kurulu ve çalışıyor\n" +
                        "✅ yt-dlp güncel sürümde\n" +
                        "✅ FFmpeg kurulu ve çalışıyor\n\n" +
                        "Program tam performansla çalışmaya hazır!", "Sistem Hazır");
                    LogMessage("🎉 Tüm sistem gereksinimleri karşılandı", Color.LimeGreen);
                }
                else
                {
                    var missingList = string.Join(", ", missingComponents);
                    statusLabel.Text = $"Eksik gereksinimler: {missingList}";
                    ShowWarning($"⚠️ Bazı gereksinimler eksik:\n\n" +
                        $"❌ {string.Join("\n❌ ", missingComponents)}\n\n" +
                        "Program çalışabilir ancak bazı özellikler sınırlı olabilir.\n\n" +
                        "Manuel kurulum için:\n" +
                        "• Python: https:
                        "• FFmpeg: winget install ffmpeg", 
                        "Eksik Gereksinimler");
                    LogMessage($"⚠️ Eksik gereksinimler: {missingList}", Color.Orange);
                }
            }
            catch (Exception ex)
            {
                ShowCriticalError($"Sistem kontrolü sırasında hata: {ex.Message}", "Hata");
                LogMessage($"❌ Sistem kontrolü hatası: {ex.Message}", Color.Red);
            }
            finally
            {
                updateButton.Enabled = true;
                downloadButton.Enabled = true;
                ShowProgress(false);
                statusLabel.Text = "Hazır";
            }
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlTextBox.Text))
            {
                ShowWarning("Lütfen bir video URL'si girin!");
                return;
            }

            if (string.IsNullOrWhiteSpace(pathTextBox.Text))
            {
                ShowWarning("Lütfen indirme yolunu belirtin!");
                return;
            }

            try
            {
                Directory.CreateDirectory(pathTextBox.Text);
            }
            catch (Exception ex)
            {
                ShowCriticalError($"İndirme klasörü oluşturulamadı: {ex.Message}");
                return;
            }

            downloadButton.Enabled = false;
            ShowProgress(true);
            downloadStartTime = DateTime.Now;

            try
            {
                await DownloadVideo();
            }
            catch (Exception ex)
            {
                ShowCriticalError($"İndirme hatası: {ex.Message}");
            }
            finally
            {
                downloadButton.Enabled = true;
                ShowProgress(false);
                statusLabel.Text = "Hazır";
            }
        }

        private async Task DownloadVideo()
        {
            var url = urlTextBox.Text.Trim();
            var outputPath = pathTextBox.Text.Trim();

            UpdateProgress(5, "İndirme başlatılıyor...");

            UpdateProgress(10, "Python kontrolü yapılıyor...");
            if (!await CheckPythonInstalled())
            {
                ShowCriticalError("Python bulunamadı!\n\nLütfen Python'u kurun ve PATH'e ekleyin.\nKurulum için: https:
                return;
            }

            UpdateProgress(20, "yt-dlp kontrolü yapılıyor...");
            if (!await CheckYtDlpInstalled())
            {
                UpdateProgress(25, "yt-dlp kuruluyor...");
                if (!await InstallYtDlp())
                {
                    ShowCriticalError("yt-dlp kurulumu başarısız!\n\nInternet bağlantınızı kontrol edin.");
                    return;
                }
            }

            UpdateProgress(35, "FFmpeg kontrolü yapılıyor...");
            if (!await CheckFFmpegInstalled())
            {
                ShowWarning("FFmpeg bulunamadı!\n\nSes birleştirme sorunları yaşanabilir.\n\nKurulum için:\n1. https:
            }

            var qualityArg = GetQualityArgument();
            var subtitleArg = subtitleCheckBox.Checked ? "--embed-subs --write-auto-sub" : "";

            var arguments = $"-m yt_dlp --no-playlist {qualityArg} {subtitleArg} --embed-thumbnail -o \"{Path.Combine(outputPath, "%(title)s.%(ext)s")}\" \"{url}\"";

            UpdateProgress(40, "Video indiriliyor...");

            var processInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    LogMessage(e.Data, System.Drawing.Color.LimeGreen);
                    ProcessDownloadOutput(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    LogMessage(e.Data, System.Drawing.Color.Orange);
                    ProcessDownloadOutput(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Run(() => process.WaitForExit());

            if (process.ExitCode == 0)
            {
                UpdateProgress(100, "İndirme tamamlandı!");
                statusLabel.Text = "Video başarıyla indirildi!";

                await CheckVideoResolution(outputPath);

                if (MessageBox.Show("Video başarıyla indirildi!\n\nİndirme klasörünü açmak ister misiniz?",
                    "Başarılı", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Process.Start("explorer.exe", outputPath);
                }
            }
            else
            {
                string errorMessage = "İndirme başarısız!\n\n";
                
                switch (process.ExitCode)
                {
                    case 1:
                        errorMessage += "Genel hata - Video URL'sini kontrol edin.";
                        break;
                    case 2:
                        errorMessage += "Video bulunamadı veya erişim engellendi.\n\n" +
                                      "Olası çözümler:\n" +
                                      "• URL'nin doğru olduğundan emin olun\n" +
                                      "• Video'nun herkese açık olduğunu kontrol edin\n" +
                                      "• Farklı bir kalite seçeneği deneyin\n" +
                                      "• yt-dlp'yi güncelleyin";
                        break;
                    case 3:
                        errorMessage += "Dosya sistemi hatası - İndirme klasörünü kontrol edin.";
                        break;
                    case 101:
                        errorMessage += "Ağ bağlantısı hatası - İnternet bağlantınızı kontrol edin.";
                        break;
                    default:
                        errorMessage += $"Hata kodu: {process.ExitCode}\n\n" +
                                      "• URL'yi kontrol edin\n" +
                                      "• yt-dlp'yi güncelleyin\n" +
                                      "• Farklı bir video deneyin";
                        break;
                }

                ShowCriticalError(errorMessage);
                statusLabel.Text = "İndirme başarısız!";
            }
        }

        private void ProcessDownloadOutput(string output)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => ProcessDownloadOutput(output));
                return;
            }

            if (output.Contains("ERROR:") || output.Contains("error:"))
            {
                if (output.Contains("Video unavailable") || output.Contains("Private video"))
                {
                    UpdateProgressText("❌ Video özel veya erişilemez");
                }
                else if (output.Contains("not found") || output.Contains("404"))
                {
                    UpdateProgressText("❌ Video bulunamadı");
                }
                else if (output.Contains("Sign in to confirm your age"))
                {
                    UpdateProgressText("❌ Yaş kısıtlaması - giriş gerekli");
                }
                else if (output.Contains("blocked"))
                {
                    UpdateProgressText("❌ Bölgede engellenen video");
                }
                else
                {
                    UpdateProgressText("⚠️ İndirme sorunu tespit edildi");
                }
                return;
            }

            if (output.Contains("[download]"))
            {
                if (output.Contains("Downloading webpage"))
                {
                    UpdateProgress(45, "Sayfa bilgileri alınıyor...");
                }
                else if (output.Contains("Downloading video info"))
                {
                    UpdateProgress(50, "Video bilgileri alınıyor...");
                }
                else if (output.Contains("Downloading m3u8 information"))
                {
                    UpdateProgress(55, "Stream bilgileri alınıyor...");
                }
            }

            if (output.Contains("%"))
            {
                var parts = output.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    if (part.Contains("%") && part.Length > 1)
                    {
                        var percentStr = part.Replace("%", "").Trim();
                        if (double.TryParse(percentStr, out double percent))
                        {
                            var adjustedPercent = 60 + (int)(percent * 0.35);
                            UpdateProgress(adjustedPercent, "İndiriliyor...");
                            break;
                        }
                    }
                }
            }

            if (output.Contains("MiB/s") || output.Contains("KiB/s"))
            {
                var speedMatch = System.Text.RegularExpressions.Regex.Match(output, @"(\d+\.?\d*)\s*(MiB/s|KiB/s)");
                if (speedMatch.Success)
                {
                    UpdateProgressText("", "", speedMatch.Value);
                }
            }

            if (output.Contains("Destination:") || output.Contains("[download]"))
            {
                var fileName = ExtractFileName(output);
                if (!string.IsNullOrEmpty(fileName))
                {
                    UpdateProgressText("", fileName);
                }
            }

            if (output.Contains("[ffmpeg]"))
            {
                UpdateProgress(95, "Video işleniyor...");
            }
        }

        private string ExtractFileName(string output)
        {
            try
            {
                if (output.Contains("Destination:"))
                {
                    var startIndex = output.IndexOf("Destination:") + "Destination:".Length;
                    var fileName = output.Substring(startIndex).Trim();
                    return Path.GetFileName(fileName);
                }
                else if (output.Contains("[download]") && output.Contains("."))
                {
                    var parts = output.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (part.Contains(".") && part.Length > 5)
                        {
                            return Path.GetFileName(part);
                        }
                    }
                }
            }
            catch { }
            return "";
        }

        private async Task<bool> CheckYtDlpInstalled()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    return true;
                }
            }
            catch
            {
            }

            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "-m yt_dlp --version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        private async Task<bool> CheckFFmpegInstalled()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    var versionLine = output.Split('\n').FirstOrDefault(l => l.Contains("ffmpeg version"));
                    return versionLine != null;
                }
            }
            catch
            {
            }

            return false;
        }

        private async Task<bool> CheckPythonInstalled()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await Task.Run(() => process.WaitForExit());

                return process.ExitCode == 0 && !string.IsNullOrEmpty(output);
            }
            catch
            {
            }

            return false;
        }

        private async Task<bool> InstallPython()
        {
            try
            {
                var storeProcessInfo = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = "install Python.Python.3.11 --silent --accept-source-agreements --accept-package-agreements",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var storeProcess = new Process { StartInfo = storeProcessInfo };
                storeProcess.Start();
                var output = await storeProcess.StandardOutput.ReadToEndAsync();
                var error = await storeProcess.StandardError.ReadToEndAsync();
                await Task.Run(() => storeProcess.WaitForExit());

                if (storeProcess.ExitCode == 0)
                {
                    await Task.Delay(5000);
                    return await CheckPythonInstalled();
                }
                else
                {
                    LogMessage($"winget kurulumu başarısız: {error}", Color.Orange);
                    
                    var webClient = new System.Net.WebClient();
                    var tempPath = Path.GetTempPath();
                    var installerPath = Path.Combine(tempPath, "python-installer.exe");

                    await webClient.DownloadFileTaskAsync(
                        "https:
                        installerPath);

                    var installerProcessInfo = new ProcessStartInfo
                    {
                        FileName = installerPath,
                        Arguments = "/quiet InstallAllUsers=1 PrependPath=1 Include_test=0",
                        UseShellExecute = true,
                        CreateNoWindow = false
                    };

                    using var installerProcess = new Process { StartInfo = installerProcessInfo };
                    installerProcess.Start();
                    await Task.Run(() => installerProcess.WaitForExit());

                    try { File.Delete(installerPath); } catch { }

                    if (installerProcess.ExitCode == 0)
                    {
                        await Task.Delay(10000);
                        return await CheckPythonInstalled();
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Python kurulumu hatası: {ex.Message}", Color.Red);
            }

            return false;
        }

        private async Task<bool> InstallFFmpeg()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "winget",
                    Arguments = "install Gyan.FFmpeg --silent --accept-source-agreements --accept-package-agreements",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode == 0)
                {
                    await Task.Delay(3000);
                    return await CheckFFmpegInstalled();
                }
                else
                {
                    LogMessage($"FFmpeg kurulumu başarısız: {error}", Color.Orange);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"FFmpeg kurulumu hatası: {ex.Message}", Color.Orange);
            }

            return false;
        }

        private async Task<bool> InstallYtDlp()
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "-m pip install --upgrade yt-dlp",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode == 0)
                {
                    await Task.Delay(2000);
                    return await CheckYtDlpInstalled();
                }
            }
            catch { }

            return false;
        }

        private string GetQualityArgument()
        {
            return qualityComboBox.SelectedIndex switch
            {
                0 => "--format \"(bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio)/best[ext=mp4]/best\"",
                1 => "--format \"(bestvideo[height<=2160][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=2160]+bestaudio)/best[height<=2160]\"",
                2 => "--format \"(bestvideo[height<=1440][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=1440]+bestaudio)/best[height<=1440]\"",
                3 => "--format \"(bestvideo[height<=1080][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=1080]+bestaudio)/best[height<=1080]\"",
                4 => "--format \"(bestvideo[height<=720][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=720]+bestaudio)/best[height<=720]\"",
                5 => "--format \"(bestvideo[height<=480][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=480]+bestaudio)/best[height<=480]\"",
                6 => "--format \"(bestvideo[height<=360][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height<=360]+bestaudio)/best[height<=360]\"",
                7 => "--extract-audio --audio-format mp3 --audio-quality 0",
                _ => "--format \"(bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio)/best\""
            };
        }

        private async Task CheckVideoResolution(string outputPath)
        {
            try
            {
                var files = Directory.GetFiles(outputPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => Path.GetExtension(f).ToLower() == ".mp4" || 
                               Path.GetExtension(f).ToLower() == ".mkv" ||
                               Path.GetExtension(f).ToLower() == ".webm" ||
                               Path.GetExtension(f).ToLower() == ".mp3")
                    .OrderByDescending(f => new FileInfo(f).CreationTime)
                    .Take(1);

                foreach (var videoFile in files)
                {
                    var fileInfo = new FileInfo(videoFile);
                    var fileSizeMB = fileInfo.Length / (1024.0 * 1024.0);
                    
                    if (fileSizeMB > 50)
                    {
                        statusLabel.Text = $"✅ Yüksek kalite video indirildi ({fileSizeMB:F1} MB)";
                    }
                    else if (fileSizeMB > 10)
                    {
                        statusLabel.Text = $"📊 Orta kalite video indirildi ({fileSizeMB:F1} MB)";
                    }
                    else
                    {
                        statusLabel.Text = $"⚠️ Düşük kalite veya ses dosyası ({fileSizeMB:F1} MB)";
                    }
                    break;
                }
            }
            catch { }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (downloadButton.Enabled == false)
            {
                var message = currentLanguage == AppLanguage.Turkish ?
                    "İndirme işlemi devam ediyor. Çıkmak istediğinizden emin misiniz?" :
                    "Download is in progress. Are you sure you want to exit?";
                var title = currentLanguage == AppLanguage.Turkish ?
                    "Çıkış Onayı" : "Exit Confirmation";

                var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }

            SaveSettings();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = Path.Combine(Application.StartupPath, "icon.ico");
                if (File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch { }

            await CheckCriticalDependencies();

            statusLabel.Text = GetText("Ready") + " - kayapater tarafından geliştirildi";
            LogMessage("Video Downloader v1.3 başlatıldı", Color.LimeGreen);
        }

        private async Task CheckCriticalDependencies()
        {
            var criticalIssues = new List<string>();

            LogMessage("Sistem gereksinimleri kontrol ediliyor...", Color.Yellow);

            var osVersion = Environment.OSVersion.Version;
            if (osVersion.Major < 10)
            {
                criticalIssues.Add("Windows 10 veya üzeri gerekli");
                LogMessage("❌ Desteklenmeyen Windows sürümü", Color.Red);
            }

            if (!await CheckPythonInstalled())
            {
                criticalIssues.Add("Python kurulu değil");
                LogMessage("⚠️ Python bulunamadı - '🔄 yt-dlp Güncelle' butonunu kullanın", Color.Orange);
            }
            else
            {
                LogMessage("✅ Python kurulu", Color.LimeGreen);
            }

            if (!await CheckYtDlpInstalled())
            {
                criticalIssues.Add("yt-dlp kurulu değil");
                LogMessage("⚠️ yt-dlp bulunamadı - '🔄 yt-dlp Güncelle' butonunu kullanın", Color.Orange);
            }
            else
            {
                LogMessage("✅ yt-dlp kurulu", Color.LimeGreen);
            }

            if (!await CheckFFmpegInstalled())
            {
                LogMessage("⚠️ FFmpeg bulunamadı - ses/video birleştirme sınırlı olabilir", Color.Orange);
            }
            else
            {
                LogMessage("✅ FFmpeg kurulu", Color.LimeGreen);
            }

            if (criticalIssues.Count > 0)
            {
                var criticalMessage = "⚠️ Program çalışması için gerekli bileşenler eksik:\n\n" +
                                    $"❌ {string.Join("\n❌ ", criticalIssues)}\n\n" +
                                    "Bu sorunları çözmek için '🔄 yt-dlp Güncelle' butonuna tıklayın.\n\n" +
                                    "Bu butona tıklamadan program sınırlı çalışacaktır.";

                MessageBox.Show(criticalMessage, "Sistem Gereksinimleri", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                statusLabel.Text = "⚠️ Gereksinimler eksik - Güncelle butonunu kullanın";
                LogMessage("⚠️ Kritik bağımlılıklar eksik - kullanıcı bilgilendirildi", Color.Orange);
            }
            else
            {
                LogMessage("✅ Tüm sistem gereksinimleri karşılandı", Color.LimeGreen);
            }
        }


    }
} 
