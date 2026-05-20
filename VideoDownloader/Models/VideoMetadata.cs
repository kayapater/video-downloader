namespace VideoDownloader.Models
{
    public class VideoMetadata
    {
        public string Title { get; set; } = "";
        public string Channel { get; set; } = "";
        public int Duration { get; set; }
        public string ThumbnailUrl { get; set; } = "";
    }
}
