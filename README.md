# Video Downloader

<div align="center">

![Video Downloader](VideoDownloader/logo.png)

**Modern and user-friendly video downloader**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2B-blue)](https://www.microsoft.com/windows)
[![Release](https://img.shields.io/badge/Release-v1.6.0-512BD4)](https://github.com/kayapater/video-downloader/releases)
[![Downloads](https://img.shields.io/github/downloads/kayapater/video-downloader/total?label=Downloads&color=blue)](https://github.com/kayapater/video-downloader/releases)

</div>

---

Video Downloader is a powerful and user-friendly Windows application that allows you to download videos from YouTube, Twitter, Instagram, TikTok, Facebook, and 50+ platforms.

### ✨ What's New in v1.6.0

#### 🎯 Full Kick.com Support
- Download Kick VODs, Clips, and Livestreams with ease.
- Improved Cloudflare bypass using `--impersonate chrome` and updated extractor arguments.

#### 🏗️ Architectural Refactoring (SOLID)
- Migrated from a monolithic structure to a modular **Service/Strategy pattern**.
- Codebase is now more maintainable, testable, and robust.

#### 🚀 UI Stability & Performance
- Better asynchronous process management preventing UI freezes.
- Improved progress tracking with accurate speed and percentage reporting.
- Smart error detection and user-friendly error messages.

#### 🔧 Other Improvements
- **Updated User-Agent** - Chrome 124 compatibility.
- **Enhanced FFmpeg Detection** - Better handling of FFmpeg in MSIX and portable environments.
- **Dependency Management** - Automated check and installation of `yt-dlp` on startup.

---

### 🎯 Features

#### 📹 Video Downloading
- 50+ platform support (YouTube, Twitter, Instagram, TikTok, Facebook, etc.)
- Multiple quality options (360p - 4K)
- Audio extraction (MP3)
- Subtitle download support
- Playlist support

#### 🎨 User Interface
- Modern and clean design
- **8 beautiful themes** (Light, Dark, Ocean, Forest, Sunset, Purple, Rose, Midnight)
- Turkish and English language support
- Simple and clear progress display
- Pause and resume downloads

#### ⚙️ Technical
- .NET 8.0 Windows Forms
- yt-dlp (Python video download module)
- FFmpeg (video/audio processing)
- Automatic dependency management

### 📦 Installation (v1.6.0+)

#### Via Microsoft Winget (Recommended)
You can install or update Video Downloader directly from the Windows Package Manager:
```powershell
winget install kayapater.VideoDownloader
```

#### Manual Installation (WiX Installer)
1. Download the latest `VideoDownloader-v1.6.0-Setup.msi` from the [Releases](https://github.com/kayapater/video-downloader/releases/latest) page.
2. Run the MSI installer.
3. The application will automatically check for required dependencies (Python, yt-dlp, FFmpeg) on first launch and guide you through installation if anything is missing.

---

### 🏗️ Architectural Refactoring (SOLID)
- **Modular Design:** Migrated from a monolithic `MainForm.cs` to a modular **Service/Strategy pattern**.
- **WiX Toolset Integration:** Switched to WiX for creating professional, Winget-compatible MSI installers.
- **Smart Dependency Management:** Improved system for detecting and auto-installing/updating `yt-dlp` (including dev versions for Kick support) and `FFmpeg`.


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
- Kick
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
