using MediaCatalog.Interfaces;
using MediaCatalog.Models;

namespace MediaCatalog.Patterns.Factories
{
    public class GameFactory : IMediaFactory
    {
        public MediaItem CreateMediaItem()
        {
            return new Game
            {
                Status = new Patterns.States.PlannedState()
            };
        }

        public string GetMediaType() => "Игра";
    }
}