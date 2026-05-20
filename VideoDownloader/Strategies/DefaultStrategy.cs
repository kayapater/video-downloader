namespace VideoDownloader.Strategies
{
    public class DefaultStrategy : IPlatformStrategy
    {
        public bool CanHandle(string url) => true; // En son fallback olarak kullanılacak

        public string GetExtraArguments(string url)
        {
            return "";
        }

        public string GetPlatformName() => "Default";
    }
}
