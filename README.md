# Video Downloader

<div align="center">

![Video Downloader](VideoDownloader/logo.png)

**Modern and user-friendly video downloader**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2B-blue)](https://www.microsoft.com/windows)
[![Release](https://img.shields.io/badge/Release-v1.3.3-green)](https://github.com/kayapater/video-downloader/releases)
[![Downloads](https://img.shields.io/github/downloads/kayapater/video-downloader/total?label=Downloads&color=blue)](https://github.com/kayapater/video-downloader/releases)

</div>

---

## 📝 Description

Video Downloader is a powerful and user-friendly Windows application that allows you to download videos from YouTube, Twitter, Instagram, TikTok, Facebook, and 50+ platforms.

### ✨ What's New in v1.3.3

#### 🔧 Installer Improvements
- ✅ **WiX Toolset** based MSI installer (official Microsoft technology)
- ✅ Full **winget** support with automatic PATH management
- ✅ Automatic upgrade support for future versions
- ✅ Clean and complete uninstall process
- ✅ Improved dependency management

### 🎯 Features

#### 📹 Video Downloading
- 50+ platform support (YouTube, Twitter, Instagram, TikTok, Facebook, etc.)
- Multiple quality options (360p - 4K)
- Audio extraction (MP3)
- Subtitle download support
- Playlist support

#### 🎨 User Interface
- Modern and clean design
- Dark and light theme
- Turkish and English language support
- Simple and clear progress display

#### ⚙️ Technical
- .NET 8.0 Windows Forms
- yt-dlp (Python video download module)
- FFmpeg (video/audio processing)
- Automatic dependency management

### 📦 Installation

#### Via Winget (Recommended)

**Install:**
```powershell
winget install kayapater.VideoDownloader
```

**Specific version:**
```powershell
winget install kayapater.VideoDownloader --version 1.3.3
```

**Upgrade:**
```powershell
winget upgrade kayapater.VideoDownloader
```

**Uninstall:**
```powershell
winget uninstall kayapater.VideoDownloader
```

**Silent installation (winget):**
```powershell
winget install kayapater.VideoDownloader --silent --accept-package-agreements --accept-source-agreements
```

#### Manual Installation

1. Download `VideoDownloader-v1.3.3-Setup.msi` from [Releases](https://github.com/kayapater/video-downloader/releases/latest)
2. Run the MSI file
3. Follow the installation wizard
4. Required dependencies will be installed automatically

**Silent installation (MSI):**
```powershell
# Completely silent
msiexec /i VideoDownloader-v1.3.3-Setup.msi /quiet /norestart

# With progress bar
msiexec /i VideoDownloader-v1.3.3-Setup.msi /passive /norestart
```

### 🛠️ Development

#### Requirements

- Visual Studio 2022 or JetBrains Rider
- .NET 8.0 SDK
- Windows 10 SDK (10.0.19041.0)

#### Building the Project

```powershell
# Clone
git clone https://github.com/kayapater/video-downloader.git
cd video-downloader

# Restore dependencies
dotnet restore

# Debug build
dotnet build -c Debug

# Release build
dotnet build -c Release

# Publish (self-contained)
dotnet publish -c Release -r win-x64 --self-contained false
```

#### Creating MSI Installer

```powershell
cd WixInstaller

# Create images
.\create-placeholder-images.ps1

# Download Python installer
Invoke-WebRequest -Uri "https://www.python.org/ftp/python/3.13.5/python-3.13.5-amd64.exe" -OutFile "python-3.13.5-amd64.exe"

# Build MSI
.\build.ps1 -DownloadPython
```

For detailed information: [WixInstaller/README.md](WixInstaller/README.md)

### 🌍 Supported Platforms

<details>
<summary>50+ Platform List (Click to expand)</summary>

#### 📺 Main Platforms
- YouTube
- Instagram
- TikTok
- Twitter/X
- Facebook
- Twitch
- Vimeo
- Dailymotion
- Reddit
- LinkedIn

#### 🎵 Music Platforms
- SoundCloud
- Bandcamp
- Mixcloud

#### 📚 Education Platforms
- Udemy
- Coursera
- Khan Academy

#### 🌏 International
- Bilibili
- Niconico
- VK
- Odnoklassniki

And many more...

</details>

### 📸 Screenshots

*(Screenshots will be added)*

### 🤝 Contributing

Contributions are welcome! Please:

1. Fork this repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

### 👤 Developer

**kayapater**

- Twitter/X: [@kayapater](https://x.com/kayapater)
- GitHub: [@kayapater](https://github.com/kayapater)

### 🙏 Acknowledgments

- [yt-dlp](https://github.com/yt-dlp/yt-dlp) - Video download engine
- [FFmpeg](https://ffmpeg.org/) - Video/audio processing
- [Newtonsoft.Json](https://www.newtonsoft.com/json) - JSON processing

---

<div align="center">

**Made with ❤️ by kayapater**

⭐ Star this repo if you find it helpful!

</div>

