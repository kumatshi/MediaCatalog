using MediaCatalog.Interfaces;
using MediaCatalog.Models;
using System.Windows.Media;

namespace MediaCatalog.Patterns.States
{
    public class InProgressState : IMediaState
    {
        public string Name => "В процессе";

        public void MarkCompleted(MediaItem item)
        {
            item.Status = new CompletedState();
        }

        public void MarkInProgress(MediaItem item)
        {
        }

        public void MarkPlanned(MediaItem item)
        {
            item.Status = new PlannedState();
        }

        public Color GetColor() => Colors.Orange;
    }
}