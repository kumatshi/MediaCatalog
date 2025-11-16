using System.Windows.Media;
using MediaCatalog.Models;

namespace MediaCatalog.Interfaces
{
    public interface IMediaState
    {
        string Name { get; }
        void MarkCompleted(MediaItem item);
        void MarkInProgress(MediaItem item);
        void MarkPlanned(MediaItem item);
        Color GetColor();
    }
}