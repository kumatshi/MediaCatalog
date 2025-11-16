using MediaCatalog.Interfaces;
using MediaCatalog.Models;
using System.Windows.Media;

namespace MediaCatalog.Patterns.States
{
    public class PlannedState : IMediaState
    {
        public string Name => "В планах";

        public void MarkCompleted(MediaItem item)
        {
            item.Status = new CompletedState();
        }

        public void MarkInProgress(MediaItem item)
        {
            item.Status = new InProgressState();
        }

        public void MarkPlanned(MediaItem item)
        {

        }

        public Color GetColor() => Colors.Gray;
    }
}