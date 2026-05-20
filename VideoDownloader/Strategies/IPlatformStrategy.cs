using System.Collections.Generic;

namespace VideoDownloader.Strategies
{
    public interface IPlatformStrategy
    {
        bool CanHandle(string url);
        string GetExtraArguments(string url);
        string GetPlatformName();
    }
}
