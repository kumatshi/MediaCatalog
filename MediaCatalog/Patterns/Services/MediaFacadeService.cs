using MediaCatalog.Interfaces;
using MediaCatalog.Models;
using MediaCatalog.Patterns.Factories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MediaCatalog.Patterns.Services
{
    public class MediaFacadeService
    {
        private ObservableCollection<MediaItem> _mediaCollection;
        private List<IMediaFactory> _factories;

        public MediaFacadeService(ObservableCollection<MediaItem> mediaCollection)
        {
            _mediaCollection = mediaCollection;
            InitializeFactories();
        }

        private void InitializeFactories()
        {
            _factories = new List<IMediaFactory>
            {
                new BookFactory(),
                new MovieFactory(),
                new GameFactory()
            };
        }
        public MediaItem CreateMedia(string mediaType)
        {
            var factory = _factories.FirstOrDefault(f => f.GetMediaType() == mediaType);
            return factory?.CreateMediaItem();
        }
        public void AddMedia(MediaItem item)
        {
            item.Id = _mediaCollection.Count + 1;
            _mediaCollection.Add(item);
        }
        public List<MediaItem> SearchMedia(string searchText)
        {
            return _mediaCollection
                .Where(item => item.Title.ToLower().Contains(searchText.ToLower()) ||
                              item.Genre.ToLower().Contains(searchText.ToLower()))
                .ToList();
        }
        public List<MediaItem> FilterByType(string mediaType)
        {
            return _mediaCollection
                .Where(item => item.GetMediaType() == mediaType)
                .ToList();
           
        }
        public void ChangeStatus(MediaItem item, string action)
        {
            switch (action)
            {
                case "Complete":
                    item.Status.MarkCompleted(item);
                    break;
                case "Start":
                    item.Status.MarkInProgress(item);
                    break;
                case "Plan":
                    item.Status.MarkPlanned(item);
                    break;
            }
        }
        public List<string> GetAvailableMediaTypes()
        {
            return _factories.Select(f => f.GetMediaType()).ToList();
        }
    }
}