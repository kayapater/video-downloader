using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace VideoDownloader
{
    public partial class MainForm : Form
    {
        private Dictionary<string, Dictionary<AppLanguage, string>> translations;
        private AppLanguage currentLanguage = AppLanguage.Turkish;
        private AppTheme currentTheme = AppTheme.Dark;  // Varsayılan dark tema
        private MenuStrip? mainMenuStrip;
        private Process? currentDownloadProcess;
        private bool isPaused = false;
        private bool isCancelled = false;
        private bool isVideoMode = true;
        private DateTime downloadStartTime;

        // Preview için
        private CancellationTokenSource? previewCancellationTokenSource;
        private readonly HttpClient httpClient = new HttpClient();
        private string lastPreviewUrl = "";

        // Modern UI Colors - Improved Palette
        private readonly Color primaryColor = Color.FromArgb(99, 102, 241);      // Indigo-500 (daha canlı)
        private readonly Color successColor = Color.FromArgb(34, 197, 94);       // Green-500 (daha parlak)
        private readonly Color dangerColor = Color.FromArgb(239, 68, 68);        // Red-500
        private readonly Color grayColor = Color.FromArgb(107, 114, 128);        // Gray-500
        private readonly Color lightBgColor = Color.FromArgb(249, 250, 251);     // Gray-50 (yumuşak beyaz)
        private readonly Color darkBgColor = Color.FromArgb(24, 24, 27);         // Zinc-900 (daha koyu, kontrastlı)

        private enum AppLanguage { Turkish, English }
        private enum AppTheme { Light, Dark }

        public MainForm()
        {
            InitializeTranslations();
            LoadSettings();
            InitializeComponent();
            InitializeDefaultValues();
            InitializeAboutMenu();
            ApplyLanguage();
            ApplyTheme();
        }

        private void InitializeDefaultValues()
        {
            // Default download path
            string defaultPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads",
                "VideoDownloader"
            );
            pathTextBox.Text = defaultPath;

            // Quality options
            qualityComboBox.Items.Clear();
            qualityComboBox.Items.AddRange(new object[]
            {
                currentLanguage == AppLanguage.Turkish ? "En İyi Kalite" : "Best Quality",
                "2160p (4K)",
                "1440p (2K)",
                "1080p (Full HD)",
                "720p (HD)",
                "480p (SD)",
                "360p"
            });
            qualityComboBox.SelectedIndex = 0;

            // Default to video mode
            isVideoMode = true;
            UpdateFormatButtons();

            // Hide progress panel initially
            progressPanel.Visible = false;
        }

        private void UpdateFormatButtons()
        {
            Color bgInput = Color.FromArgb(39, 39, 42);      // Zinc-800 (daha parlak)
            Color textMuted = Color.FromArgb(212, 212, 216); // Zinc-300 (daha okunabilir)
            Color primaryBtn = Color.FromArgb(99, 102, 241); // Indigo-500

            if (isVideoMode)
            {
                videoFormatButton.BackColor = primaryBtn;
                videoFormatButton.ForeColor = Color.White;
                audioFormatButton.BackColor = bgInput;
                audioFormatButton.ForeColor = textMuted;

                qualityComboBox.Enabled = true;
                subtitleCheckBox.Enabled = true;
            }
            else
            {
                audioFormatButton.BackColor = primaryBtn;
                audioFormatButton.ForeColor = Color.White;
                videoFormatButton.BackColor = bgInput;
                videoFormatButton.ForeColor = textMuted;

                qualityComboBox.Enabled = false;
                subtitleCheckBox.Enabled = false;
                subtitleCheckBox.Checked = false;
            }
        }

        private void ChangeLanguage(AppLanguage language)
        {
            currentLanguage = language;
            ApplyLanguage();
            SaveSettings();
        }

        private void ChangeTheme(AppTheme theme)
        {
            currentTheme = theme;
            ApplyTheme();
            SaveSettings();
        }

        private void ApplyLanguage()
        {
            this.Text = GetText("FormTitle");
            urlLabel.Text = GetText("VideoURL");
            pasteButton.Text = "📋";
            videoFormatButton.Text = GetText("VideoOption");
            audioFormatButton.Text = GetText("AudioOption");
            qualityLabel.Text = GetText("Quality");
            subtitleCheckBox.Text = GetText("DownloadSubtitles");
            pathLabel.Text = GetText("DownloadPath");
            browseButton.Text = GetText("Browse");
            downloadButton.Text = GetText("Download");
            cancelButton.Text = GetText("Cancel");
            statusLabel.Text = GetText("Ready");

            // Update quality items
            var selectedIndex = qualityComboBox.SelectedIndex;
            qualityComboBox.Items.Clear();
            qualityComboBox.Items.AddRange(new object[]
            {
                currentLanguage == AppLanguage.Turkish ? "En İyi Kalite" : "Best Quality",
                "2160p (4K)",
                "1440p (2K)",
                "1080p (Full HD)",
                "720p (HD)",
                "480p (SD)",
                "360p"
            });
            if (selectedIndex >= 0 && selectedIndex < qualityComboBox.Items.Count)
                qualityComboBox.SelectedIndex = selectedIndex;
            else
                qualityComboBox.SelectedIndex = 0;

            // Placeholder for URL
            if (string.IsNullOrWhiteSpace(urlTextBox.Text) ||
                urlTextBox.Text == translations["URLPlaceholder"][AppLanguage.Turkish] ||
                urlTextBox.Text == translations["URLPlaceholder"][AppLanguage.English])
            {
                urlTextBox.Text = "";
                urlTextBox.ForeColor = grayColor;
            }

            // Menüyü güncelle
            UpdateMenuLanguage();
        }

        private void UpdateMenuLanguage()
        {
            if (mainMenuStrip == null || mainMenuStrip.Items.Count < 2) return;

            // Ayarlar menüsü
            var settingsMenu = mainMenuStrip.Items[0] as ToolStripMenuItem;
            if (settingsMenu != null)
            {
                settingsMenu.Text = GetText("Settings");

                // Dil alt menüsü
                if (settingsMenu.DropDownItems.Count > 0)
                {
                    var languageMenu = settingsMenu.DropDownItems[0] as ToolStripMenuItem;
                    if (languageMenu != null)
                    {
                        languageMenu.Text = GetText("Language");
                        if (languageMenu.DropDownItems.Count >= 2)
                        {
                            languageMenu.DropDownItems[0].Text = GetText("Turkish");
                            languageMenu.DropDownItems[1].Text = GetText("English");
                        }
                    }
                }

                // Tema alt menüsü
                if (settingsMenu.DropDownItems.Count > 1)
                {
                    var themeMenu = settingsMenu.DropDownItems[1] as ToolStripMenuItem;
                    if (themeMenu != null)
                    {
                        themeMenu.Text = GetText("Theme");
                        if (themeMenu.DropDownItems.Count >= 2)
                        {
                            themeMenu.DropDownItems[0].Text = GetText("LightTheme");
                            themeMenu.DropDownItems[1].Text = GetText("DarkTheme");
                        }
                    }
                }

                // Sistem Kontrolü menü öğesi (index 3, separator'dan sonra)
                if (settingsMenu.DropDownItems.Count > 3)
                {
                    settingsMenu.DropDownItems[3].Text = GetText("SystemCheck");
                }
            }

            // Yardım menüsü
            var helpMenu = mainMenuStrip.Items[1] as ToolStripMenuItem;
            if (helpMenu != null)
            {
                helpMenu.Text = GetText("Help");
                if (helpMenu.DropDownItems.Count >= 2)
                {
                    helpMenu.DropDownItems[0].Text = GetText("About");
                    helpMenu.DropDownItems[1].Text = GetText("SupportedSites");
                }
            }
        }

        private void ApplyTheme()
        {
            Color backgroundColor, foregroundColor, inputBackColor, inputForeColor, panelColor;

            if (currentTheme == AppTheme.Dark)
            {
                // Dark Theme - Improved readability
                backgroundColor = darkBgColor;                      // Zinc-900: #18181B
                foregroundColor = Color.FromArgb(250, 250, 250);    // Zinc-50 (daha parlak)
                inputBackColor = Color.FromArgb(39, 39, 42);        // Zinc-800 (daha açık input)
                inputForeColor = Color.FromArgb(244, 244, 245);     // Zinc-100 (net okuma)
                panelColor = Color.FromArgb(39, 39, 42);            // Zinc-800
            }
            else
            {
                // Light Theme - Improved contrast
                backgroundColor = lightBgColor;                      // Gray-50: #F9FAFB
                foregroundColor = Color.FromArgb(24, 24, 27);       // Zinc-900 (koyu metin)
                inputBackColor = Color.White;                       // Beyaz input
                inputForeColor = Color.FromArgb(24, 24, 27);        // Zinc-900 (net okuma)
                panelColor = Color.White;                           // Beyaz panel
            }

            this.BackColor = backgroundColor;

            foreach (Control control in this.Controls)
            {
                ApplyThemeToControl(control, backgroundColor, foregroundColor, inputBackColor, inputForeColor, panelColor);
            }
        }

        private void ApplyThemeToControl(Control control, Color bgColor, Color fgColor, Color inputBg, Color inputFg, Color panelColor)
        {
            if (control is Label label && control.Name != "statusLabel")
            {
                label.ForeColor = fgColor;
            }
            else if (control is TextBox textBox)
            {
                textBox.BackColor = inputBg;
                textBox.ForeColor = inputFg;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BackColor = inputBg;
                comboBox.ForeColor = inputFg;
            }
            else if (control is CheckBox checkBox)
            {
                checkBox.ForeColor = fgColor;
            }
            else if (control is Panel panel)
            {
                if (panel.Name == "progressPanel" || panel.Name == "previewPanel")
                {
                    panel.BackColor = panelColor;
                }

                foreach (Control child in panel.Controls)
                {
                    ApplyThemeToControl(child, bgColor, fgColor, inputBg, inputFg, panelColor);
                }
            }
            else if (control is MenuStrip menuStrip)
            {
                menuStrip.BackColor = bgColor;
                menuStrip.ForeColor = fgColor;
                foreach (ToolStripItem item in menuStrip.Items)
                {
                    item.BackColor = bgColor;
                    item.ForeColor = fgColor;
                }
            }
        }

        private void InitializeTranslations()
        {
            translations = new Dictionary<string, Dictionary<AppLanguage, string>>
            {
                ["FormTitle"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Video İndirici",
                    [AppLanguage.English] = "Video Downloader"
                },
                ["VideoURL"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Video URL",
                    [AppLanguage.English] = "Video URL"
                },
                ["URLPlaceholder"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "YouTube, Twitter, Instagram linkini yapıştırın...",
                    [AppLanguage.English] = "Paste YouTube, Twitter, Instagram link..."
                },
                ["Quality"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Kalite",
                    [AppLanguage.English] = "Quality"
                },
                ["DownloadSubtitles"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Altyazı İndir",
                    [AppLanguage.English] = "Download Subtitles"
                },
                ["DownloadPath"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "İndirme Klasörü",
                    [AppLanguage.English] = "Download Folder"
                },
                ["Browse"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Gözat",
                    [AppLanguage.English] = "Browse"
                },
                ["Download"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "⬇️ İndir",
                    [AppLanguage.English] = "⬇️ Download"
                },
                ["Ready"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Hazır",
                    [AppLanguage.English] = "Ready"
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
                    [AppLanguage.Turkish] = "Video İndirici v1.4.0\n\nYouTube, Twitter ve Instagram'dan video indirme aracı",
                    [AppLanguage.English] = "Video Downloader v1.4.0\n\nDownload videos from YouTube, Twitter and Instagram"
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
                ["SupportedSites"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Desteklenen Siteler",
                    [AppLanguage.English] = "Supported Sites"
                },
                ["Cancel"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "İptal",
                    [AppLanguage.English] = "Cancel"
                },
                ["Downloading"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "İndiriliyor...",
                    [AppLanguage.English] = "Downloading..."
                },
                ["Completed"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Tamamlandı!",
                    [AppLanguage.English] = "Completed!"
                },
                ["Error"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Hata",
                    [AppLanguage.English] = "Error"
                },
                ["Success"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Başarılı",
                    [AppLanguage.English] = "Success"
                },
                ["SystemCheck"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Sistem Kontrolü",
                    [AppLanguage.English] = "System Check"
                },
                ["CheckingDependencies"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Bağımlılıklar kontrol ediliyor...",
                    [AppLanguage.English] = "Checking dependencies..."
                },
                ["InstallingYtDlp"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "yt-dlp kuruluyor...",
                    [AppLanguage.English] = "Installing yt-dlp..."
                },
                ["YtDlpInstalled"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "yt-dlp başarıyla kuruldu!",
                    [AppLanguage.English] = "yt-dlp installed successfully!"
                },
                ["YtDlpInstallFailed"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "yt-dlp kurulumu başarısız!",
                    [AppLanguage.English] = "yt-dlp installation failed!"
                },
                ["PythonNotFound"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "Python bulunamadı! Lütfen python.org adresinden Python kurun.",
                    [AppLanguage.English] = "Python not found! Please install Python from python.org"
                },
                ["AllDependenciesOk"] = new Dictionary<AppLanguage, string>
                {
                    [AppLanguage.Turkish] = "✓ Tüm bağımlılıklar hazır",
                    [AppLanguage.English] = "✓ All dependencies ready"
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
            catch { }
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
            catch { }
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

            // Sistem Kontrolü menü öğesi
            var systemCheckMenuItem = new ToolStripMenuItem(GetText("SystemCheck"));
            systemCheckMenuItem.Click += SystemCheckMenuItem_Click;

            settingsMenu.DropDownItems.AddRange(new ToolStripItem[] { languageMenu, themeMenu, new ToolStripSeparator(), systemCheckMenuItem });

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
            Color backgroundColor = currentTheme == AppTheme.Dark ? darkBgColor : lightBgColor;
            Color foregroundColor = currentTheme == AppTheme.Dark ? Color.FromArgb(243, 244, 246) : Color.FromArgb(17, 24, 39);

            var aboutForm = new Form
            {
                Text = GetText("AboutTitle"),
                Size = new Size(450, 400),
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
                Size = new Size(400, 60),
                Font = new Font("Segoe UI", 11),
                ForeColor = foregroundColor
            };

            var developerLabel = new Label
            {
                Text = GetText("Developer"),
                Location = new Point(20, 90),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 10),
                ForeColor = foregroundColor
            };

            var kayapaterLink = new LinkLabel
            {
                Text = "kayapater",
                Location = new Point(105, 90),
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
                catch { }
            };

            var featuresText = currentLanguage == AppLanguage.Turkish ?
                @"Bu uygulama ile:
• YouTube, Twitter, Instagram videoları
• TikTok, Facebook, Vimeo ve daha fazlası
• Farklı kalite seçenekleri (4K, 1080p, 720p)
• MP3 olarak ses indirme
• Altyazı indirme desteği

Teknolojiler: .NET 8.0, yt-dlp, FFmpeg" :
                @"With this app:
• YouTube, Twitter, Instagram videos
• TikTok, Facebook, Vimeo and more
• Different quality options (4K, 1080p, 720p)
• Download audio as MP3
• Subtitle download support

Technologies: .NET 8.0, yt-dlp, FFmpeg";

            var featuresLabel = new Label
            {
                Text = featuresText,
                Location = new Point(20, 120),
                Size = new Size(400, 180),
                Font = new Font("Segoe UI", 9),
                ForeColor = foregroundColor
            };

            var okButton = new Button
            {
                Text = GetText("OK"),
                Location = new Point(340, 320),
                Size = new Size(80, 35),
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9),
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 0;

            aboutForm.Controls.AddRange(new Control[] { textLabel, developerLabel, kayapaterLink, featuresLabel, okButton });
            aboutForm.AcceptButton = okButton;
            aboutForm.ShowDialog(this);
        }

        private void SupportedSitesMenuItem_Click(object sender, EventArgs e)
        {
            Color backgroundColor = currentTheme == AppTheme.Dark ? darkBgColor : lightBgColor;
            Color foregroundColor = currentTheme == AppTheme.Dark ? Color.FromArgb(243, 244, 246) : Color.FromArgb(17, 24, 39);

            var sitesForm = new Form
            {
                Text = GetText("SupportedSites"),
                Size = new Size(500, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = backgroundColor
            };

            var sitesText = currentLanguage == AppLanguage.Turkish ?
                @"📺 ANA PLATFORMLAR:
YouTube, Instagram, TikTok, Twitter/X
Facebook, Twitch, Vimeo, Dailymotion
Reddit, LinkedIn

🔞 YETİŞKİN (+18):
Pornhub, XVideos, RedTube ve diğerleri

📺 TV & HABER:
BBC iPlayer, CNN, ESPN, Arte

🌏 ULUSLARARASI:
Bilibili, Niconico, VK

🎵 MÜZİK:
SoundCloud, Bandcamp, Mixcloud

📚 EĞİTİM:
Udemy, Coursera, Khan Academy

Ve 1000+ site daha..." :
                @"📺 MAIN PLATFORMS:
YouTube, Instagram, TikTok, Twitter/X
Facebook, Twitch, Vimeo, Dailymotion
Reddit, LinkedIn

🔞 ADULT (+18):
Pornhub, XVideos, RedTube and others

📺 TV & NEWS:
BBC iPlayer, CNN, ESPN, Arte

🌏 INTERNATIONAL:
Bilibili, Niconico, VK

🎵 MUSIC:
SoundCloud, Bandcamp, Mixcloud

📚 EDUCATION:
Udemy, Coursera, Khan Academy

And 1000+ more sites...";

            var sitesLabel = new Label
            {
                Text = sitesText,
                Location = new Point(20, 20),
                Size = new Size(440, 380),
                Font = new Font("Segoe UI", 10),
                ForeColor = foregroundColor
            };

            var okButton = new Button
            {
                Text = GetText("OK"),
                Location = new Point(390, 420),
                Size = new Size(80, 35),
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9),
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 0;

            sitesForm.Controls.AddRange(new Control[] { sitesLabel, okButton });
            sitesForm.AcceptButton = okButton;
            sitesForm.ShowDialog(this);
        }

        private async void SystemCheckMenuItem_Click(object sender, EventArgs e)
        {
            Color backgroundColor = currentTheme == AppTheme.Dark ? darkBgColor : lightBgColor;
            Color foregroundColor = currentTheme == AppTheme.Dark ? Color.FromArgb(243, 244, 246) : Color.FromArgb(17, 24, 39);
            Color successColor = Color.FromArgb(34, 197, 94);  // Green
            Color errorColor = Color.FromArgb(239, 68, 68);    // Red
            Color warningColor = Color.FromArgb(234, 179, 8);  // Yellow

            var checkForm = new Form
            {
                Text = GetText("SystemCheck"),
                Size = new Size(450, 350),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = backgroundColor
            };

            var titleLabel = new Label
            {
                Text = currentLanguage == AppLanguage.Turkish ? "🔧 Sistem Durumu" : "🔧 System Status",
                Location = new Point(20, 20),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = foregroundColor
            };

            var pythonLabel = new Label
            {
                Text = currentLanguage == AppLanguage.Turkish ? "⏳ Python kontrol ediliyor..." : "⏳ Checking Python...",
                Location = new Point(20, 70),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = foregroundColor
            };

            var ytdlpLabel = new Label
            {
                Text = currentLanguage == AppLanguage.Turkish ? "⏳ yt-dlp kontrol ediliyor..." : "⏳ Checking yt-dlp...",
                Location = new Point(20, 100),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = foregroundColor
            };

            var ffmpegLabel = new Label
            {
                Text = currentLanguage == AppLanguage.Turkish ? "⏳ FFmpeg kontrol ediliyor..." : "⏳ Checking FFmpeg...",
                Location = new Point(20, 130),
                Size = new Size(400, 25),
                Font = new Font("Segoe UI", 10),
                ForeColor = foregroundColor
            };

            var statusLabel = new Label
            {
                Text = "",
                Location = new Point(20, 180),
                Size = new Size(400, 50),
                Font = new Font("Segoe UI", 9),
                ForeColor = foregroundColor
            };

            var installButton = new Button
            {
                Text = currentLanguage == AppLanguage.Turkish ? "yt-dlp Kur" : "Install yt-dlp",
                Location = new Point(20, 250),
                Size = new Size(120, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Visible = false
            };
            installButton.FlatAppearance.BorderSize = 0;

            var closeButton = new Button
            {
                Text = GetText("OK"),
                Location = new Point(340, 250),
                Size = new Size(80, 35),
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 9),
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            closeButton.FlatAppearance.BorderSize = 0;

            checkForm.Controls.AddRange(new Control[] { titleLabel, pythonLabel, ytdlpLabel, ffmpegLabel, statusLabel, installButton, closeButton });
            checkForm.AcceptButton = closeButton;

            // Kontrolleri başlat
            checkForm.Shown += async (s, args) =>
            {
                // Python kontrolü
                var pythonOk = await CheckPythonInstalled();
                pythonLabel.Text = pythonOk 
                    ? (currentLanguage == AppLanguage.Turkish ? "✅ Python kurulu" : "✅ Python installed")
                    : (currentLanguage == AppLanguage.Turkish ? "❌ Python bulunamadı" : "❌ Python not found");
                pythonLabel.ForeColor = pythonOk ? successColor : errorColor;

                if (!pythonOk)
                {
                    statusLabel.Text = currentLanguage == AppLanguage.Turkish 
                        ? "Python gerekli! python.org adresinden indirin." 
                        : "Python required! Download from python.org";
                    statusLabel.ForeColor = errorColor;
                    return;
                }

                // yt-dlp kontrolü
                var ytdlpOk = await CheckYtDlpInstalled();
                ytdlpLabel.Text = ytdlpOk 
                    ? (currentLanguage == AppLanguage.Turkish ? "✅ yt-dlp kurulu" : "✅ yt-dlp installed")
                    : (currentLanguage == AppLanguage.Turkish ? "❌ yt-dlp bulunamadı" : "❌ yt-dlp not found");
                ytdlpLabel.ForeColor = ytdlpOk ? successColor : errorColor;

                if (!ytdlpOk)
                {
                    installButton.Visible = true;
                    statusLabel.Text = currentLanguage == AppLanguage.Turkish 
                        ? "yt-dlp kurulu değil. Kurmak için butona tıklayın." 
                        : "yt-dlp not installed. Click button to install.";
                    statusLabel.ForeColor = warningColor;
                }

                // FFmpeg kontrolü
                var ffmpegOk = await CheckFFmpegInstalled();
                ffmpegLabel.Text = ffmpegOk 
                    ? (currentLanguage == AppLanguage.Turkish ? "✅ FFmpeg kurulu" : "✅ FFmpeg installed")
                    : (currentLanguage == AppLanguage.Turkish ? "⚠️ FFmpeg bulunamadı (opsiyonel)" : "⚠️ FFmpeg not found (optional)");
                ffmpegLabel.ForeColor = ffmpegOk ? successColor : warningColor;

                if (pythonOk && ytdlpOk)
                {
                    statusLabel.Text = currentLanguage == AppLanguage.Turkish 
                        ? "✅ Tüm gerekli bağımlılıklar hazır!" 
                        : "✅ All required dependencies ready!";
                    statusLabel.ForeColor = successColor;
                }
            };

            // yt-dlp kur butonu
            installButton.Click += async (s, args) =>
            {
                installButton.Enabled = false;
                installButton.Text = currentLanguage == AppLanguage.Turkish ? "Kuruluyor..." : "Installing...";
                statusLabel.Text = currentLanguage == AppLanguage.Turkish ? "yt-dlp kuruluyor, lütfen bekleyin..." : "Installing yt-dlp, please wait...";
                statusLabel.ForeColor = foregroundColor;

                var installed = await InstallYtDlp();
                
                if (installed)
                {
                    ytdlpLabel.Text = currentLanguage == AppLanguage.Turkish ? "✅ yt-dlp kurulu" : "✅ yt-dlp installed";
                    ytdlpLabel.ForeColor = successColor;
                    statusLabel.Text = currentLanguage == AppLanguage.Turkish ? "✅ yt-dlp başarıyla kuruldu!" : "✅ yt-dlp installed successfully!";
                    statusLabel.ForeColor = successColor;
                    installButton.Visible = false;
                }
                else
                {
                    statusLabel.Text = currentLanguage == AppLanguage.Turkish ? "❌ yt-dlp kurulumu başarısız!" : "❌ yt-dlp installation failed!";
                    statusLabel.ForeColor = errorColor;
                    installButton.Text = currentLanguage == AppLanguage.Turkish ? "Tekrar Dene" : "Retry";
                    installButton.Enabled = true;
                }
            };

            checkForm.ShowDialog(this);
        }

        // Event Handlers
        private void PasteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    var clipboardText = Clipboard.GetText().Trim();
                    if (Uri.TryCreate(clipboardText, UriKind.Absolute, out var uri) &&
                        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                    {
                        urlTextBox.Text = clipboardText;
                        urlTextBox.ForeColor = currentTheme == AppTheme.Dark ?
                            Color.FromArgb(243, 244, 246) : Color.FromArgb(17, 24, 39);
                    }
                }
            }
            catch { }
        }

        private void UrlTextBox_TextChanged(object sender, EventArgs e)
        {
            var url = urlTextBox.Text.Trim();

            // Geçerli URL mi kontrol et
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                // Aynı URL için tekrar sorgu yapma
                if (url != lastPreviewUrl)
                {
                    lastPreviewUrl = url;
                    _ = LoadVideoPreviewAsync(url);
                }
            }
            else
            {
                // Geçersiz URL, önizlemeyi gizle
                HidePreview();
                lastPreviewUrl = "";
            }
        }

        private async Task LoadVideoPreviewAsync(string url)
        {
            // Önceki işlemi iptal et
            previewCancellationTokenSource?.Cancel();
            previewCancellationTokenSource = new CancellationTokenSource();
            var token = previewCancellationTokenSource.Token;

            try
            {
                // UI'ı yükleniyor moduna al
                ShowPreviewLoading();

                // yt-dlp ile video bilgilerini al
                var videoInfo = await GetVideoInfoAsync(url, token);

                if (token.IsCancellationRequested) return;

                if (videoInfo != null)
                {
                    // Önizleme bilgilerini göster
                    ShowPreview(videoInfo);
                }
                else
                {
                    HidePreview();
                }
            }
            catch (OperationCanceledException)
            {
                // İptal edildi, normal
            }
            catch (Exception)
            {
                HidePreview();
            }
        }

        private async Task<VideoInfo?> GetVideoInfoAsync(string url, CancellationToken token)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var ytDlpPath = GetYtDlpPath();
                    if (string.IsNullOrEmpty(ytDlpPath)) return null;

                    // FFmpeg location
                    string ffmpegLocationArg = "";
                    if (File.Exists(Path.Combine(Application.StartupPath, "ffmpeg.exe")))
                    {
                        ffmpegLocationArg = $"--ffmpeg-location \"{Application.StartupPath}\"";
                    }

                    // Kick ve Twitch için özel User-Agent ve headerlar
                    var extraArgs = "";
                    if (url.Contains("kick.com") || url.Contains("twitch.tv"))
                    {
                        extraArgs = "--user-agent \"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36\" --referer \"" + url + "\"";
                    }

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = $"-m yt_dlp {extraArgs} {ffmpegLocationArg} --no-download --print-json \"{url}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = System.Text.Encoding.UTF8
                    };

                    using var process = new Process { StartInfo = startInfo };
                    process.Start();

                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(15000); // 15 saniye timeout

                    if (token.IsCancellationRequested) return null;

                    if (!string.IsNullOrEmpty(output))
                    {
                        using var doc = JsonDocument.Parse(output);
                        var root = doc.RootElement;

                        // Thumbnail URL'i al
                        string thumbnailUrl = "";
                        
                        // 1. Önce thumbnails array'inden en iyi JPG'yi bulmaya çalış (Windows Forms WebP sevmeyebilir)
                        if (root.TryGetProperty("thumbnails", out var thumbnails) && thumbnails.ValueKind == JsonValueKind.Array)
                        {
                            var thumbList = thumbnails.EnumerateArray().ToList();
                            
                            // JPG olanları bul
                            var jpgThumbs = thumbList.Where(t => 
                                t.TryGetProperty("url", out var u) && 
                                (u.GetString()?.Contains(".jpg") == true)).ToList();

                            if (jpgThumbs.Any())
                            {
                                // JPG'lerin en sonuncusu (genelde en yüksek kalite)
                                var bestJpg = jpgThumbs.Last();
                                if (bestJpg.TryGetProperty("url", out var url)) 
                                    thumbnailUrl = url.GetString() ?? "";
                            }
                            else if (thumbList.Any())
                            {
                                // JPG yoksa listenin en sonuncusu (WebP olabilir)
                                var last = thumbList.Last();
                                if (last.TryGetProperty("url", out var url)) 
                                    thumbnailUrl = url.GetString() ?? "";
                            }
                        }
                        
                        // 2. Eğer hala boşsa, root 'thumbnail' alanına bak
                        if (string.IsNullOrEmpty(thumbnailUrl) && root.TryGetProperty("thumbnail", out var thumb))
                        {
                            thumbnailUrl = thumb.GetString() ?? "";
                        }

                        var info = new VideoInfo
                        {
                            Title = root.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                            Channel = root.TryGetProperty("uploader", out var uploader) ? uploader.GetString() ?? "" :
                                     (root.TryGetProperty("channel", out var channel) ? channel.GetString() ?? "" : ""),
                            Duration = root.TryGetProperty("duration", out var duration) && duration.ValueKind == JsonValueKind.Number ? duration.GetInt32() : 0,
                            ThumbnailUrl = thumbnailUrl
                        };

                        return info;
                    }
                }
                catch { }
                return null;
            }, token);
        }

        private void ShowPreviewLoading()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ShowPreviewLoading));
                return;
            }

            // Önce formu genişlet
            ExpandFormForPreview();

            previewPanel.Visible = true;
            thumbnailPictureBox.Visible = true; // Thumbnail alanını göster (siyah kutu)
            thumbnailPictureBox.Image = null;   // Önce temizle
            videoTitleLabel.Visible = false;
            videoChannelLabel.Visible = false;
            videoDurationLabel.Visible = false;
            previewLoadingLabel.Visible = true;
            previewLoadingLabel.Text = currentLanguage == AppLanguage.Turkish ?
                "Video bilgileri yükleniyor..." : "Loading video info...";
        }

        private void ShowPreview(VideoInfo info)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowPreview(info)));
                return;
            }

            previewLoadingLabel.Visible = false;
            
            // Tüm kontrolleri görünür yap
            thumbnailPictureBox.Visible = true;
            videoTitleLabel.Visible = true;
            videoChannelLabel.Visible = true;
            videoDurationLabel.Visible = true;

            // Başlık - AutoEllipsis ile otomatik kısalt
            videoTitleLabel.Text = info.Title ?? "";

            // Kanal - güvenli kontrol
            videoChannelLabel.Text = !string.IsNullOrWhiteSpace(info.Channel) ? info.Channel : "Bilinmeyen Kanal";

            // Süre
            if (info.Duration > 0)
            {
                var span = TimeSpan.FromSeconds(info.Duration);
                videoDurationLabel.Text = span.Hours > 0
                    ? $"⏱ {span:hh\\:mm\\:ss}"
                    : $"⏱ {span:mm\\:ss}";
            }
            else
            {
                videoDurationLabel.Text = "";
            }

            // Thumbnail varsa yükle, yoksa boş bırak
            if (!string.IsNullOrWhiteSpace(info.ThumbnailUrl))
            {
                System.Diagnostics.Debug.WriteLine($"Thumbnail URL: {info.ThumbnailUrl}");
                _ = LoadThumbnailAsync(info.ThumbnailUrl);
            }
            else
            {
                thumbnailPictureBox.Image = null;
                System.Diagnostics.Debug.WriteLine("Thumbnail URL boş!");
            }

            previewPanel.Visible = true;
        }

        private async Task LoadThumbnailAsync(string thumbnailUrl)
        {
            if (string.IsNullOrWhiteSpace(thumbnailUrl)) return;

            try
            {
                // WebP dosyaları Windows Forms'da desteklenmiyor, skip et
                if (thumbnailUrl.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine("WebP thumbnail skipped - not supported by Windows Forms");
                    // Placeholder olarak boş bırak, video bilgileri yine de gösterilecek
                    return;
                }

                // Byte array olarak indir (Stream sorunlarını önlemek için)
                var imageBytes = await httpClient.GetByteArrayAsync(thumbnailUrl);
                
                using (var ms = new MemoryStream(imageBytes))
                using (var tempImage = Image.FromStream(ms))
                {
                    // Bitmap kopyası oluştur (Stream bağımlılığını koparmak için)
                    var safeImage = new Bitmap(tempImage);
                    
                    if (IsDisposed || !IsHandleCreated) return;

                    Invoke(new Action(() =>
                    {
                        try
                        {
                            var oldImage = thumbnailPictureBox.Image;
                            thumbnailPictureBox.Image = safeImage;
                            oldImage?.Dispose();
                            thumbnailPictureBox.Refresh();
                        }
                        catch
                        {
                            safeImage?.Dispose();
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Thumbnail error: {ex.Message}");
            }
        }

        private void HidePreview()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(HidePreview));
                return;
            }

            if (previewPanel.Visible)
            {
                previewPanel.Visible = false;
                CollapseFormFromPreview();
            }
        }

        private void ExpandFormForPreview()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ExpandFormForPreview));
                return;
            }

            // Zaten genişlemiş mi kontrol et
            if (this.ClientSize.Height > 520) return;

            // Önizleme paneli görünür olacak, diğer kontrolleri aşağı kaydır (115px)
            qualityLabel.Location = new Point(30, 260);
            qualityComboBox.Location = new Point(30, 290);
            subtitleCheckBox.Location = new Point(300, 297);
            pathLabel.Location = new Point(30, 345);
            pathTextBox.Location = new Point(30, 375);
            browseButton.Location = new Point(625, 373);
            downloadButton.Location = new Point(30, 435);
            progressPanel.Location = new Point(30, 505);

            // Form boyutunu büyüt
            this.MinimumSize = new Size(788, 620);
            this.MaximumSize = new Size(788, 620);
            this.ClientSize = new Size(770, 575);
        }

        private void CollapseFormFromPreview()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(CollapseFormFromPreview));
                return;
            }

            // Kontrolleri orijinal konumlarına geri getir
            qualityLabel.Location = new Point(30, 145);
            qualityComboBox.Location = new Point(30, 175);
            subtitleCheckBox.Location = new Point(300, 182);
            pathLabel.Location = new Point(30, 230);
            pathTextBox.Location = new Point(30, 260);
            browseButton.Location = new Point(625, 258);
            downloadButton.Location = new Point(30, 320);
            progressPanel.Location = new Point(30, 390);

            // Form boyutunu küçült
            this.MinimumSize = new Size(788, 505);
            this.MaximumSize = new Size(788, 505);
            this.ClientSize = new Size(770, 458);
        }

        private class VideoInfo
        {
            public string Title { get; set; } = "";
            public string Channel { get; set; } = "";
            public int Duration { get; set; }
            public string ThumbnailUrl { get; set; } = "";
        }

        private void VideoFormatButton_Click(object sender, EventArgs e)
        {
            isVideoMode = true;
            UpdateFormatButtons();
        }

        private void AudioFormatButton_Click(object sender, EventArgs e)
        {
            isVideoMode = false;
            UpdateFormatButtons();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();
            folderDialog.Description = currentLanguage == AppLanguage.Turkish ?
                "İndirilen videoların kaydedileceği klasörü seçin" :
                "Select the folder to save downloaded videos";
            folderDialog.SelectedPath = pathTextBox.Text;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CancelDownload();
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

                    UpdateProgress(0, currentLanguage == AppLanguage.Turkish ? "İptal edildi" : "Cancelled");
                    progressPanel.Visible = false;

                    downloadButton.Enabled = true;
                    downloadButton.Text = GetText("Download");
                    statusLabel.Text = GetText("Ready");
                }
            }
            catch { }
        }

        private async void DownloadButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(urlTextBox.Text))
            {
                ShowWarning(currentLanguage == AppLanguage.Turkish ?
                    "Lütfen bir video URL'si girin!" :
                    "Please enter a video URL!");
                return;
            }

            if (string.IsNullOrWhiteSpace(pathTextBox.Text))
            {
                ShowWarning(currentLanguage == AppLanguage.Turkish ?
                    "Lütfen indirme yolunu belirtin!" :
                    "Please specify download path!");
                return;
            }

            try
            {
                Directory.CreateDirectory(pathTextBox.Text);
            }
            catch (Exception ex)
            {
                ShowCriticalError(currentLanguage == AppLanguage.Turkish ?
                    $"İndirme klasörü oluşturulamadı: {ex.Message}" :
                    $"Could not create download folder: {ex.Message}");
                return;
            }

            downloadButton.Enabled = false;
            progressPanel.Visible = true;
            
            // Form yüksekliğini artır (progress panel için)
            if (this.ClientSize.Height < 490)
            {
                this.MinimumSize = new Size(788, 532);
                this.MaximumSize = new Size(788, 532);
                this.ClientSize = new Size(770, 485);
            }
            
            downloadStartTime = DateTime.Now;

            try
            {
                await DownloadVideo();
            }
            catch (Exception ex)
            {
                ShowCriticalError($"{GetText("Error")}: {ex.Message}");
            }
            finally
            {
                downloadButton.Enabled = true;
                progressPanel.Visible = false;
                
                // Form yüksekliğini küçült
                if (!previewPanel.Visible && this.ClientSize.Height > 470)
                {
                    this.MinimumSize = new Size(788, 505);
                    this.MaximumSize = new Size(788, 505);
                    this.ClientSize = new Size(770, 458);
                }
                
                statusLabel.Text = GetText("Ready");
            }
        }

        private async Task DownloadVideo()
        {
            var url = urlTextBox.Text.Trim();
            var outputPath = pathTextBox.Text.Trim();

            UpdateProgress(5, currentLanguage == AppLanguage.Turkish ? "Başlatılıyor..." : "Starting...");

            // Check Python
            UpdateProgress(10, currentLanguage == AppLanguage.Turkish ? "Python kontrol ediliyor..." : "Checking Python...");
            if (!await CheckPythonInstalled())
            {
                ShowCriticalError(currentLanguage == AppLanguage.Turkish ?
                    "Python bulunamadı!\n\nLütfen Python'u kurun: https://www.python.org/downloads/" :
                    "Python not found!\n\nPlease install Python: https://www.python.org/downloads/");
                return;
            }

            // Check yt-dlp
            UpdateProgress(20, currentLanguage == AppLanguage.Turkish ? "yt-dlp kontrol ediliyor..." : "Checking yt-dlp...");
            if (!await CheckYtDlpInstalled())
            {
                UpdateProgress(25, currentLanguage == AppLanguage.Turkish ? "yt-dlp kuruluyor..." : "Installing yt-dlp...");
                if (!await InstallYtDlp())
                {
                    ShowCriticalError(currentLanguage == AppLanguage.Turkish ?
                        "yt-dlp kurulumu başarısız!" :
                        "yt-dlp installation failed!");
                    return;
                }
            }

            // Check FFmpeg
            UpdateProgress(30, currentLanguage == AppLanguage.Turkish ? "FFmpeg kontrol ediliyor..." : "Checking FFmpeg...");
            bool ffmpegInstalled = await CheckFFmpegInstalled();
            string ffmpegLocationArg = "";
            
            if (File.Exists(Path.Combine(Application.StartupPath, "ffmpeg.exe")))
            {
                ffmpegLocationArg = $"--ffmpeg-location \"{Application.StartupPath}\"";
            }
            else if (!ffmpegInstalled && (url.Contains("twitch.tv") || url.Contains("kick.com")))
            {
                 ShowWarning(currentLanguage == AppLanguage.Turkish ? 
                    "FFmpeg bulunamadı! Twitch ve Kick indirmeleri için FFmpeg gereklidir.\nLütfen ffmpeg.exe'yi uygulama klasörüne atın." :
                    "FFmpeg not found! FFmpeg is required for Twitch and Kick downloads.\nPlease place ffmpeg.exe in the application folder.");
            }

            // Build arguments
            var qualityArg = GetQualityArgument();
            var subtitleArg = subtitleCheckBox.Checked ? "--embed-subs --write-auto-sub" : "";
            
            // Kick ve Twitch için özel argümanlar
            var extraArgs = "";
            if (url.Contains("kick.com"))
            {
                extraArgs = "--user-agent \"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36\" --referer \"" + url + "\"";
            }
            else if (url.Contains("twitch.tv"))
            {
                // Twitch HLS download hatası için ffmpeg downloader kullan
                extraArgs = "--downloader ffmpeg";
            }

            var arguments = $"-m yt_dlp {extraArgs} {ffmpegLocationArg} --no-playlist {qualityArg} {subtitleArg} --embed-thumbnail --merge-output-format mp4 -o \"{Path.Combine(outputPath, "%(title)s.%(ext)s")}\" \"{url}\"";

            UpdateProgress(0, GetText("Downloading"));

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

            cancelButton.Enabled = true;

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data) && !isCancelled)
                {
                    ProcessDownloadOutput(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data) && !isCancelled)
                {
                    ProcessDownloadOutput(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await Task.Run(() => process.WaitForExit());

            cancelButton.Enabled = false;
            currentDownloadProcess = null;

            if (process.ExitCode == 0)
            {
                UpdateProgress(100, GetText("Completed"));
                statusLabel.Text = isVideoMode ?
                    (currentLanguage == AppLanguage.Turkish ? "Video başarıyla indirildi!" : "Video downloaded successfully!") :
                    (currentLanguage == AppLanguage.Turkish ? "Ses başarıyla indirildi!" : "Audio downloaded successfully!");

                var message = currentLanguage == AppLanguage.Turkish ?
                    "İndirme tamamlandı!\n\nKlasörü açmak ister misiniz?" :
                    "Download complete!\n\nWould you like to open the folder?";

                if (MessageBox.Show(message, GetText("Success"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Process.Start("explorer.exe", outputPath);
                }
            }
            else
            {
                var errorMessage = currentLanguage == AppLanguage.Turkish ?
                    "İndirme başarısız!\n\nURL'yi kontrol edin ve tekrar deneyin." :
                    "Download failed!\n\nCheck the URL and try again.";
                ShowCriticalError(errorMessage);
                statusLabel.Text = currentLanguage == AppLanguage.Turkish ? "İndirme başarısız!" : "Download failed!";
            }
        }

        private void ProcessDownloadOutput(string output)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => ProcessDownloadOutput(output));
                return;
            }

            // Debug: statusLabel'a her çıktıyı göster
            if (output.Contains("[download]") || output.Contains("[ffmpeg]"))
            {
                statusLabel.Text = output.Length > 80 ? output.Substring(0, 80) + "..." : output;
            }

            // Parse progress percentage - yt-dlp format: "[download]   0.1% of  227.22MiB"
            if (output.Contains("%") && output.Contains("[download]"))
            {
                // Regex ile yüzdeyi bul
                var match = System.Text.RegularExpressions.Regex.Match(output, @"(\d+\.?\d*)\s*%");
                if (match.Success)
                {
                    if (double.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double percent))
                    {
                        UpdateProgress((int)Math.Round(percent), GetText("Downloading"));
                    }
                }
            }

            // Parse speed
            if (output.Contains("MiB/s") || output.Contains("KiB/s"))
            {
                var speedMatch = System.Text.RegularExpressions.Regex.Match(output, @"(\d+\.?\d*)\s*(MiB/s|KiB/s)");
                if (speedMatch.Success)
                {
                    speedLabel.Text = $"⚡ {speedMatch.Value}";
                }
            }

            // FFmpeg processing
            if (output.Contains("[ffmpeg]"))
            {
                UpdateProgress(95, currentLanguage == AppLanguage.Turkish ? "İşleniyor..." : "Processing...");
            }
        }

        private void UpdateProgress(int percentage, string status)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateProgress(percentage, status));
                return;
            }

            if (percentage >= 0 && percentage <= 100)
            {
                progressBar.Value = percentage;
                progressLabel.Text = $"{percentage}%";
            }

            if (!string.IsNullOrEmpty(status))
            {
                statusLabel.Text = status;
            }
        }

        private string GetQualityArgument()
        {
            if (!isVideoMode)
            {
                return "--extract-audio --audio-format mp3 --audio-quality 0";
            }

            return qualityComboBox.SelectedIndex switch
            {
                0 => "--format \"bestvideo+bestaudio/best\"",
                1 => "--format \"bestvideo[height<=2160]+bestaudio/best[height<=2160]/best\"",
                2 => "--format \"bestvideo[height<=1440]+bestaudio/best[height<=1440]/best\"",
                3 => "--format \"bestvideo[height<=1080]+bestaudio/best[height<=1080]/best\"",
                4 => "--format \"bestvideo[height<=720]+bestaudio/best[height<=720]/best\"",
                5 => "--format \"bestvideo[height<=480]+bestaudio/best[height<=480]/best\"",
                6 => "--format \"bestvideo[height<=360]+bestaudio/best[height<=360]/best\"",
                _ => "--format \"bestvideo+bestaudio/best\""
            };
        }

        private string GetSelectedQuality()
        {
            if (qualityComboBox.SelectedIndex < 0) return currentLanguage == AppLanguage.Turkish ? "En İyi" : "Best";

            return qualityComboBox.SelectedIndex switch
            {
                0 => currentLanguage == AppLanguage.Turkish ? "En İyi" : "Best",
                1 => "2160p (4K)",
                2 => "1440p (2K)",
                3 => "1080p (Full HD)",
                4 => "720p (HD)",
                5 => "480p (SD)",
                6 => "360p",
                _ => currentLanguage == AppLanguage.Turkish ? "En İyi" : "Best"
            };
        }

        // Dependency checks
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
            catch { return false; }
        }

        private async Task<bool> CheckYtDlpInstalled()
        {
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

                return process.ExitCode == 0 && !string.IsNullOrEmpty(output);
            }
            catch { return false; }
        }

        private string? GetYtDlpPath()
        {
            // Python module olarak yt-dlp kullanıyoruz
            // Bu metot python executable path'ini döndürür
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = "python",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processInfo };
                process.Start();
                var output = process.StandardOutput.ReadLine();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(output) && File.Exists(output))
                {
                    return "python";
                }
            }
            catch { }
            return null;
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

        private async Task<bool> CheckFFmpegInstalled()
        {
            try
            {
                // 1. Check local folder
                if (File.Exists(Path.Combine(Application.StartupPath, "ffmpeg.exe"))) return true;

                // 2. Check PATH
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
                await Task.Run(() => process.WaitForExit());

                return process.ExitCode == 0;
            }
            catch { return false; }
        }

        private void ShowCriticalError(string message)
        {
            MessageBox.Show(message, GetText("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowWarning(string message)
        {
            var title = currentLanguage == AppLanguage.Turkish ? "Uyarı" : "Warning";
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!downloadButton.Enabled)
            {
                var message = currentLanguage == AppLanguage.Turkish ?
                    "İndirme işlemi devam ediyor. Çıkmak istediğinizden emin misiniz?" :
                    "Download is in progress. Are you sure you want to exit?";
                var title = currentLanguage == AppLanguage.Turkish ? "Çıkış Onayı" : "Exit Confirmation";

                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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

            statusLabel.Text = GetText("Ready");

            // Başlangıçta yt-dlp kontrol et ve eksikse otomatik kur
            await CheckAndInstallDependenciesOnStartup();
        }

        private async Task CheckAndInstallDependenciesOnStartup()
        {
            try
            {
                statusLabel.Text = GetText("CheckingDependencies");

                // Python kontrolü
                var pythonOk = await CheckPythonInstalled();
                if (!pythonOk)
                {
                    statusLabel.Text = GetText("PythonNotFound");
                    return;
                }

                // yt-dlp kontrolü
                var ytdlpOk = await CheckYtDlpInstalled();
                if (!ytdlpOk)
                {
                    statusLabel.Text = GetText("InstallingYtDlp");
                    
                    var installed = await InstallYtDlp();
                    if (installed)
                    {
                        statusLabel.Text = GetText("YtDlpInstalled");
                        await Task.Delay(2000);
                        statusLabel.Text = GetText("Ready");
                    }
                    else
                    {
                        statusLabel.Text = GetText("YtDlpInstallFailed");
                    }
                }
                else
                {
                    statusLabel.Text = GetText("Ready");
                }
            }
            catch
            {
                statusLabel.Text = GetText("Ready");
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+F = Focus URL TextBox
            if (e.Control && e.KeyCode == Keys.F)
            {
                urlTextBox.Focus();
                urlTextBox.SelectAll();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            // Ctrl+V = Yapıştır
            else if (e.Control && e.KeyCode == Keys.V)
            {
                PasteButton_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            // Enter = İndir
            else if (e.KeyCode == Keys.Enter && downloadButton.Enabled)
            {
                DownloadButton_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void videoChannelLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
