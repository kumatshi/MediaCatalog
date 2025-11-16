using MediaCatalog.Interfaces;
using MediaCatalog.Models;
using System.Windows.Media;

namespace MediaCatalog.Patterns.States
{
    public class CompletedState : IMediaState
    {
        public string Name => "Завершено";

        public void MarkCompleted(MediaItem item)
        {
        }

        public void MarkInProgress(MediaItem item)
        {
            item.Status = new InProgressState();
        }

        public void MarkPlanned(MediaItem item)
        {
            item.Status = new PlannedState();
        }

        public Color GetColor() => Colors.Green;
    }
}