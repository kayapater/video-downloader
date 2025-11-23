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
        private bool isVideoMode = true;
        private Process currentDownloadProcess;
        private bool isPaused = false;
        private bool isCancelled = false;

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
            qualityComboBox.SelectedIndex = 0;
            videoRadioButton.Checked = true;
            isVideoMode = true;
            pathTextBox.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "Video Downloader");
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
            mediaTypeLabel.Text = GetText("MediaType");
            videoRadioButton.Text = GetText("VideoOption");
            audioRadioButton.Text = GetText("AudioOption");
            qualityLabel.Text = GetText("Quality");
            pathLabel.Text = GetText("DownloadPath");
            urlTextBox.PlaceholderText = GetText("URLPlaceholder");
            browseButton.Text = GetText("Browse");
            downloadButton.Text = GetText("Download");
            updateButton.Text = GetText("SystemCheck");
            subtitleCheckBox.Text = GetText("DownloadSubtitles");
            progressGroupBox.Text = GetText("DownloadStatus");
            pauseResumeButton.Text = GetText("Pause");
            cancelButton.Text = GetText("Cancel");
            UpdateComboBoxItems();
            RefreshMenuStrip();
            if (statusLabel.Text.Contains("Hazır") || statusLabel.Text.Contains("Ready"))
            {
                statusLabel.Text = GetText("Ready") + " - " + GetText("DevelopedBy");
            }
        }

        private void UpdateComboBoxItems()
        {
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
                "360p"
            });
            qualityComboBox.SelectedIndex = qualitySelection >= 0 ? qualitySelection : 0;
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
                else if (control is RadioButton radioButton)
                {
                    radioButton.BackColor = backgroundColor;
                    radioButton.ForeColor = foregroundColor;
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
                        else if (innerControl is Button innerButton)
                        {
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
                    [AppLanguage.Turkish] = "YouTube, Twitter, Instagram veya diğer platform linklerini buraya yapıştırın...",
                    [AppLanguage.English] = "Paste YouTube, Twitter, Instagram or other platform links here..."
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
                ["DevelopedBy"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "kayapater tarafından geliştirildi",
                    [AppLanguage.English] = "developed by kayapater"
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
                    [AppLanguage.Turkish] = "Video İndirici v1.3.2\n\nYouTube, Twitter ve Instagram'dan video indirme aracı",
                    [AppLanguage.English] = "Video Downloader v1.3.2\n\nDownload videos from YouTube, Twitter and Instagram"
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

                ["MediaType"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Medya Türü:",
                    [AppLanguage.English] = "Media Type:"
                },
                ["VideoOption"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "🎬 Video",
                    [AppLanguage.English] = "🎬 Video"
                },
                ["AudioOption"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "🎵 Ses",
                    [AppLanguage.English] = "🎵 Audio"
                },
                ["Download"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "🎬 İndir",
                    [AppLanguage.English] = "🎬 Download"
                },
                ["SupportedSites"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Desteklenen Siteler",
                    [AppLanguage.English] = "Supported Sites"
                },
                ["Pause"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "⏸️ Duraklat",
                    [AppLanguage.English] = "⏸️ Pause"
                },
                ["Resume"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "▶️ Devam Et",
                    [AppLanguage.English] = "▶️ Resume"
                },
                ["Cancel"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "❌ İptal",
                    [AppLanguage.English] = "❌ Cancel"
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
            var supportedSitesMenuItem = new ToolStripMenuItem(GetText("SupportedSites"));

            aboutMenuItem.Click += AboutMenuItem_Click;
            supportedSitesMenuItem.Click += SupportedSitesMenuItem_Click;
            
            helpMenu.DropDownItems.Add(aboutMenuItem);
            helpMenu.DropDownItems.Add(supportedSitesMenuItem);

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
                Size = new Size(520, 550),
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
                    Process.Start(new ProcessStartInfo("https://github.com/kayapater") { UseShellExecute = true });
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
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 9),
                ForeColor = foregroundColor,
                AutoSize = true,
                MaximumSize = new Size(450, 0)
            };

            var scrollPanel = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(460, 320),
                AutoScroll = true,
                BackColor = backgroundColor
            };

            scrollPanel.Controls.Add(featuresLabel);

            var okButton = new Button
            {
                Text = GetText("OK"),
                Location = new Point(410, 470),
                Size = new Size(80, 35),
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            aboutForm.Controls.AddRange(new Control[] { textLabel, developerLabel, kayapaterLink, scrollPanel, okButton });
            aboutForm.AcceptButton = okButton;
            aboutForm.ShowDialog(this);
        }

        private void SupportedSitesMenuItem_Click(object sender, EventArgs e)
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

            var sitesForm = new Form
            {
                Text = GetText("SupportedSites"),
                Size = new Size(680, 650),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = backgroundColor
            };

            var sitesText = currentLanguage == AppLanguage.Turkish ?
                @"🎬 VİDEO SİTELERİ:

📺 ANA PLATFORMLAR:
• YouTube 
• Instagram 
• TikTok 
• Twitter/X 
• Facebook 
• Twitch 
• Vimeo 
• Dailymotion 
• Reddit 
• LinkedIn 

🔞 YETİŞKİN PLATFORMLAR +18:
• Pornhub 
• XVideos 
• RedTube 
• YouPorn 
• Xhamster 

📺 TV & HABER:
• BBC iPlayer 
• CNN 
• ESPN 
• Arte 
• CNBC 

🌏 ULUSLARARASI:
• Bilibili 
• Niconico 
• VK 
• Odnoklassniki 

🎵 MÜZİK & SES:
• SoundCloud 
• Bandcamp 
• Mixcloud 

📚 EĞİTİM & KURSLAR:
• Udemy 
• Coursera 
• Khan Academy" :
                @"🎬 VIDEO SITES:

📺 MAIN PLATFORMS:
• YouTube 
• Instagram 
• TikTok 
• Twitter/X 
• Facebook 
• Twitch 
• Vimeo 
• Dailymotion 
• Reddit 
• LinkedIn 

🔞 ADULT PLATFORMS +18: 
• Pornhub 
• XVideos 
• RedTube 
• YouPorn 
• Xhamster 

📺 TV & NEWS:
• BBC iPlayer 
• CNN 
• ESPN 
• Arte 
• CNBC 

🌏 INTERNATIONAL:
• Bilibili 
• Niconico 
• VK 
• Odnoklassniki 

🎵 MUSIC & AUDIO:
• SoundCloud 
• Bandcamp 
• Mixcloud 

📚 EDUCATION & COURSES:
• Udemy 
• Coursera 
• Khan Academy";

            var sitesLabel = new Label
            {
                Text = sitesText,
                Location = new Point(10, 10),
                Font = new Font("Segoe UI", 9),
                ForeColor = foregroundColor,
                AutoSize = true,
                MaximumSize = new Size(580, 0)
            };

            var scrollPanel = new Panel
            {
                Location = new Point(20, 20),
                Size = new Size(620, 520),
                AutoScroll = true,
                BackColor = backgroundColor
            };

            scrollPanel.Controls.Add(sitesLabel);

            var okButton = new Button
            {
                Text = GetText("OK"),
                Location = new Point(580, 560),
                Size = new Size(80, 35),
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            sitesForm.Controls.AddRange(new Control[] { scrollPanel, okButton });
            sitesForm.AcceptButton = okButton;
            sitesForm.ShowDialog(this);
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

                pauseResumeButton.Enabled = false;
                cancelButton.Enabled = false;
            }
            else
            {

                pauseResumeButton.Enabled = false;
                cancelButton.Enabled = false;
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

        private void VideoRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (videoRadioButton.Checked)
            {
                isVideoMode = true;
                qualityComboBox.Enabled = true;
                subtitleCheckBox.Enabled = true;
            }
        }

        private void AudioRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (audioRadioButton.Checked)
            {
                isVideoMode = false;
                qualityComboBox.Enabled = false;
                subtitleCheckBox.Enabled = false;
                subtitleCheckBox.Checked = false;
            }
        }

        private void PauseResumeButton_Click(object sender, EventArgs e)
        {
            if (currentDownloadProcess == null || currentDownloadProcess.HasExited)
                return;

            if (isPaused)
            {

                ResumeDownload();
            }
            else
            {

                PauseDownload();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CancelDownload();
        }

        private void PauseDownload()
        {
            try
            {
                if (currentDownloadProcess != null && !currentDownloadProcess.HasExited)
                {


                    isPaused = true;
                    pauseResumeButton.Text = GetText("Resume");
                    pauseResumeButton.BackColor = Color.FromArgb(40, 167, 69); // Green
                    
                    LogMessage("İndirme duraklatıldı", Color.Orange);
                    UpdateProgress(-1, "Duraklatıldı...");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Duraklat hatası: {ex.Message}", Color.Red);
            }
        }

        private void ResumeDownload()
        {
            try
            {
                isPaused = false;
                pauseResumeButton.Text = GetText("Pause");
                pauseResumeButton.BackColor = Color.FromArgb(255, 193, 7); // Yellow
                
                LogMessage("İndirme devam ediyor", Color.LimeGreen);
                UpdateProgress(-1, "Devam ediyor...");
                



            }
            catch (Exception ex)
            {
                LogMessage($"Devam ettirme hatası: {ex.Message}", Color.Red);
            }
        }

        private void CancelDownload()
        {
            try
            {
                if (currentDownloadProcess != null && !currentDownloadProcess.HasExited)
                {
                    isCancelled = true;
                    currentDownloadProcess.Kill();
                    currentDownloadProcess = null;
                    
                    LogMessage("İndirme iptal edildi", Color.Red);
                    UpdateProgress(0, "İptal edildi");
                    ShowProgress(false);
                    

                    downloadButton.Enabled = true;
                    downloadButton.Text = GetText("Download");
                    statusLabel.Text = GetText("Ready");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"İptal hatası: {ex.Message}", Color.Red);
            }
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
                        "• Python: https://www.python.org/downloads/\n" +
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
                ShowCriticalError("Python bulunamadı!\n\nLütfen Python'u kurun ve PATH'e ekleyin.\nKurulum için: https://www.python.org/downloads/");
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
                ShowWarning("FFmpeg bulunamadı!\n\nSes birleştirme sorunları yaşanabilir.\n\nKurulum için:\n1. https://ffmpeg.org/download.html\n2. winget install ffmpeg");
            }

            bool useSelectedQuality = true;
            var qualityArg = GetQualityArgument();
            var subtitleArg = subtitleCheckBox.Checked ? "--embed-subs --write-auto-sub" : "";

            var arguments = $"-m yt_dlp --no-playlist {qualityArg} {subtitleArg} --embed-thumbnail --verbose -o \"{Path.Combine(outputPath, "%(title)s.%(ext)s")}\" \"{url}\"";

            if (qualityComboBox.SelectedIndex > 0 && isVideoMode)
            {
                UpdateProgress(37, "Seçilen kalite kontrol ediliyor...");
                bool qualityExists = await CheckIfQualityExists(url, qualityArg);
                
                if (!qualityExists)
                {
                    var selectedQuality = GetSelectedQuality();
                    var message = currentLanguage == AppLanguage.Turkish
                        ? $"Seçilen kalite ({selectedQuality}) bu videoda mevcut değil.\n\nEn yüksek kalitede indirmeye devam edilsin mi?"
                        : $"Selected quality ({selectedQuality}) is not available for this video.\n\nContinue with highest available quality?";
                    
                    var title = currentLanguage == AppLanguage.Turkish ? "Kalite Bulunamadı" : "Quality Not Found";
                    
                    var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        qualityArg = "--format \"(bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio)/best[ext=mp4]/best\"";
                        arguments = $"-m yt_dlp --no-playlist {qualityArg} {subtitleArg} --embed-thumbnail --verbose -o \"{Path.Combine(outputPath, "%(title)s.%(ext)s")}\" \"{url}\"";
                        useSelectedQuality = false;
                    }
                    else
                    {
                        UpdateProgress(0, "İndirme iptal edildi");
                        statusLabel.Text = currentLanguage == AppLanguage.Turkish ? "İndirme iptal edildi" : "Download cancelled";
                        return;
                    }
                }
            }

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
            currentDownloadProcess = process;
            isCancelled = false;
            isPaused = false;

            pauseResumeButton.Text = GetText("Pause");
            pauseResumeButton.BackColor = Color.FromArgb(255, 193, 7);
            pauseResumeButton.Enabled = true;
            cancelButton.Enabled = true;

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data) && !isCancelled)
                {
                    if (!IsDebugMessage(e.Data))
                    {
                        LogMessage(e.Data, System.Drawing.Color.LimeGreen);
                        ProcessDownloadOutput(e.Data);
                    }
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data) && !isCancelled)
                {
                    if (!IsDebugMessage(e.Data))
                    {
                        LogMessage(e.Data, System.Drawing.Color.Orange);
                        ProcessDownloadOutput(e.Data);
                    }
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Run(() => process.WaitForExit());

            pauseResumeButton.Enabled = false;
            cancelButton.Enabled = false;
            currentDownloadProcess = null;

            if (process.ExitCode == 0)
            {
                UpdateProgress(100, "İndirme tamamlandı!");
                statusLabel.Text = isVideoMode ? "Video başarıyla indirildi!" : "Ses başarıyla indirildi!";

                await CheckVideoResolution(outputPath);

                var message = isVideoMode ? 
                    "Video başarıyla indirildi!\n\nİndirme klasörünü açmak ister misiniz?" :
                    "Ses dosyası başarıyla indirildi!\n\nİndirme klasörünü açmak ister misiniz?";
                    
                if (MessageBox.Show(message, "Başarılı", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
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



        private string GetSelectedQuality()
        {
            if (qualityComboBox.SelectedIndex < 0) return "En İyi";
            
            return qualityComboBox.SelectedIndex switch
            {
                0 => currentLanguage == AppLanguage.Turkish ? "En İyi" : "Best",
                1 => "2160p (4K)",
                2 => "1440p (2K)",
                3 => "1080p (Full HD)",
                4 => "720p (HD)",
                5 => "480p (SD)",
                6 => currentLanguage == AppLanguage.Turkish ? "360p (Düşük)" : "360p (Low)",
                _ => currentLanguage == AppLanguage.Turkish ? "En İyi" : "Best"
            };
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
                // PATH'i yeniden oku (Registry'den)
                RefreshEnvironmentPath();
                
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
        
        private void RefreshEnvironmentPath()
        {
            try
            {
                // Sistem PATH'ini oku
                var machinePath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment", "Path", "") as string;
                
                // Kullanıcı PATH'ini oku
                var userPath = Registry.GetValue(@"HKEY_CURRENT_USER\Environment", "Path", "") as string;
                
                // Birleştir ve mevcut process'e uygula
                var combinedPath = machinePath;
                if (!string.IsNullOrEmpty(userPath))
                {
                    combinedPath = combinedPath + ";" + userPath;
                }
                
                Environment.SetEnvironmentVariable("PATH", combinedPath, EnvironmentVariableTarget.Process);
            }
            catch (Exception ex)
            {
                LogMessage($"PATH yenileme hatası: {ex.Message}", Color.Orange);
            }
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
                        "https://www.python.org/ftp/python/3.11.0/python-3.11.0-amd64.exe",
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

        private async Task<bool> CheckIfQualityExists(string url, string qualityArg)
        {
            try
            {
                var arguments = $"-m yt_dlp --list-formats --no-playlist \"{url}\"";

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
                process.Start();
                
                var output = await process.StandardOutput.ReadToEndAsync();
                await Task.Run(() => process.WaitForExit());

                if (process.ExitCode != 0)
                {
                    return false;
                }

                var selectedHeight = GetSelectedHeightFromIndex(qualityComboBox.SelectedIndex);
                if (selectedHeight == 0) return true;

                // Daha kapsamlı kalite kontrolü
                var lines = output.Split('\n');
                foreach (var line in lines)
                {
                    // Farklı format gösterimlerini kontrol et
                    // Örnek: "1920x1080", "1080p", "x1080", "height=1080"
                    if (line.Contains($"{selectedHeight}p") || 
                        line.Contains($"x{selectedHeight}") || 
                        line.Contains($"{selectedHeight}x") ||
                        line.Contains($"height={selectedHeight}") ||
                        System.Text.RegularExpressions.Regex.IsMatch(line, $@"\b{selectedHeight}\b"))
                    {
                        LogMessage($"✅ Kalite bulundu: {selectedHeight}p - {line.Trim()}", Color.LimeGreen);
                        return true;
                    }
                }

                LogMessage($"⚠️ Kalite bulunamadı: {selectedHeight}p", Color.Orange);
                return false;
            }
            catch (Exception ex)
            {
                LogMessage($"Kalite kontrolü hatası: {ex.Message}", Color.Orange);
                return true; // Hata durumunda indirmeye devam et
            }
        }

        private int GetSelectedHeightFromIndex(int index)
        {
            return index switch
            {
                0 => 0,
                1 => 2160,
                2 => 1440,
                3 => 1080,
                4 => 720,
                5 => 480,
                6 => 360,
                _ => 0
            };
        }


        private string GetQualityArgument()
        {
            if (!isVideoMode)
            {
                return "--extract-audio --audio-format mp3 --audio-quality 0";
            }
            
            // Kalite seçimi: Önce tam istenen kaliteyi dene, yoksa en yakınını al
            // Format: bestvideo[height=X] = tam X yüksekliğinde en iyi video
            // Fallback: bestvideo[height<=X] = X'e kadar olan en iyi
            return qualityComboBox.SelectedIndex switch
            {
                0 => "--format \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio/best\"",
                1 => "--format \"bestvideo[height=2160][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height=2160]+bestaudio/bestvideo[height<=2160][ext=mp4]+bestaudio/best\"",
                2 => "--format \"bestvideo[height=1440][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height=1440]+bestaudio/bestvideo[height<=1440][ext=mp4]+bestaudio/best\"",
                3 => "--format \"bestvideo[height=1080][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height=1080]+bestaudio/bestvideo[height<=1080][ext=mp4]+bestaudio/best\"",
                4 => "--format \"bestvideo[height=720][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height=720]+bestaudio/bestvideo[height<=720][ext=mp4]+bestaudio/best\"",
                5 => "--format \"bestvideo[height=480][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height=480]+bestaudio/bestvideo[height<=480][ext=mp4]+bestaudio/best\"",
                6 => "--format \"bestvideo[height=360][ext=mp4]+bestaudio[ext=m4a]/bestvideo[height=360]+bestaudio/bestvideo[height<=360][ext=mp4]+bestaudio/best\"",
                _ => "--format \"bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio/best\""
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
                    var fileName = Path.GetFileName(videoFile);

                    // FFprobe ile gerçek çözünürlüğü kontrol et
                    string actualResolution = await GetVideoResolution(videoFile);
                    
                    if (!string.IsNullOrEmpty(actualResolution))
                    {
                        var selectedQuality = GetSelectedQuality();
                        statusLabel.Text = $"✅ Video indirildi: {actualResolution} ({fileSizeMB:F1} MB)";
                        LogMessage($"İndirilen video: {fileName}", Color.LimeGreen);
                        LogMessage($"Seçilen kalite: {selectedQuality}", Color.Cyan);
                        LogMessage($"Gerçek çözünürlük: {actualResolution}", Color.Cyan);
                        LogMessage($"Dosya boyutu: {fileSizeMB:F1} MB", Color.Cyan);
                    }
                    else
                    {
                        // FFprobe yoksa dosya boyutuna göre tahmin et
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
                    }
                    break;
                }
            }
            catch { }
        }

        private async Task<string> GetVideoResolution(string videoPath)
        {
            try
            {
                // FFprobe kullanarak video çözünürlüğünü al
                var processInfo = new ProcessStartInfo
                {
                    FileName = "ffprobe",
                    Arguments = $"-v error -select_streams v:0 -show_entries stream=width,height -of csv=s=x:p=0 \"{videoPath}\"",
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
                    var resolution = output.Trim();
                    // Örnek output: "1920x1080"
                    if (resolution.Contains("x"))
                    {
                        var parts = resolution.Split('x');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int height))
                        {
                            return $"{resolution} ({height}p)";
                        }
                        return resolution;
                    }
                }
            }
            catch
            {
                // FFprobe yoksa veya hata varsa boş string dön
            }
            return "";
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

            statusLabel.Text = GetText("Ready") + " - " + GetText("DevelopedBy");
            LogMessage("Video Downloader v1.3.2 başlatıldı", Color.LimeGreen);
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

        private bool IsDebugMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return true;
            
            var debugKeywords = new[]
            {
                "[debug]",
                "[Debug]",
                "DEBUG:",
                "debug:",
                "[urllib3.connectionpool]",
                "[requests.packages.urllib3]",
                "Starting new HTTPS connection",
                "Starting new HTTP connection", 
                "Resetting dropped connection:",
                "Connection pool is full",
                "certificate verify failed",
                "InsecurePlatformWarning",
                "SNIMissingWarning",
                "InsecureRequestWarning",
                "urllib3.disable_warnings",
                "requests.packages.urllib3.disable_warnings",
                "Traceback (most recent call last):",
                "File \"<frozen",
                "line ",
                "    ",
                "\t",
                "WARNING: ",
                "UserWarning:",
                "DeprecationWarning:",
                "FutureWarning:",
                "ResourceWarning:",
                "[core/audio_conversion]",
                "[core/video_conversion]", 
                "[ffmpeg/audio]",
                "[ffmpeg/video]",
                "[generic]",
                "[extractor]",
                "[cookies]",
                "[downloader]",
                "[postprocessor]",
                "Selected format:",
                "Requested format:",
                "Available formats:",
                "format code",
                "extension",
                "resolution",
                "note"
            };

            foreach (var keyword in debugKeywords)
            {
                if (message.Contains(keyword))
                {
                    return true;
                }
            }

            if (message.Trim().Length < 5)
            {
                return true;
            }

            return false;
        }
    }
}