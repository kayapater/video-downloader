namespace VideoDownloader.Strategies
{
    public class TwitchStrategy : IPlatformStrategy
    {
        public bool CanHandle(string url) => url.Contains("twitch.tv");

        public string GetExtraArguments(string url)
        {
            return "--downloader ffmpeg " +
                   "--user-agent \"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36\" " +
                   "--referer \"https://www.twitch.tv/\" ";
        }

        public string GetPlatformName() => "Twitch";
    }
}
