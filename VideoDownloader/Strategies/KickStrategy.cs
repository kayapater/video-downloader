namespace VideoDownloader.Strategies
{
    public class KickStrategy : IPlatformStrategy
    {
        public bool CanHandle(string url) => url.Contains("kick.com");

        public string GetExtraArguments(string url)
        {
            // Kick v1.6.0 - Latest yt-dlp compatibility
            // Impersonate chrome is essential for Cloudflare bypass
            return "--impersonate chrome " +
                   "--no-check-certificates " +
                   "--hls-prefer-native " +
                   "--concurrent-fragments 5 " +
                   "--extractor-args \"kick:api_host=kick.com\" ";
        }

        public string GetPlatformName() => "Kick";
    }
}
