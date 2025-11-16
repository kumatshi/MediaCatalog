using MediaCatalog.Models;

namespace MediaCatalog.Interfaces
{
    public interface IMediaFactory
    {
        MediaItem CreateMediaItem();
        string GetMediaType();
    }
}