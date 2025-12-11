using MediaCatalog.Interfaces;
using MediaCatalog.Models;
using MediaCatalog.Patterns.Factories;
using MediaCatalog.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MediaCatalog.Patterns.Services
{
    /// <summary>
    /// Фасадный сервис для управления медиа-каталогом
    /// </summary>
    public class MediaFacadeService
    {
        private readonly ObservableCollection<MediaItem> _mediaCollection;
        private readonly List<IMediaFactory> _factories;
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// Инициализирует новый экземпляр MediaFacadeService
        /// </summary>
        public MediaFacadeService(ObservableCollection<MediaItem> mediaCollection)
        {
            _mediaCollection = mediaCollection ?? throw new ArgumentNullException(nameof(mediaCollection));
            _databaseService = new DatabaseService();

            _databaseService.InitializeDatabase();
            _factories = InitializeFactories();
            LoadDataFromDatabase();
        }

        /// <summary>
        /// Загружает данные из базы данных
        /// </summary>
        private void LoadDataFromDatabase()
        {
            try
            {
                var items = _databaseService.LoadMediaItems();
                _mediaCollection.Clear();
                foreach (var item in items)
                {
                    _mediaCollection.Add(item);
                }
            }
            catch (Exception ex)
            {
                HandleError("Ошибка загрузки данных из базы", ex);
            }
        }

        /// <summary>
        /// Инициализирует фабрики для создания медиа-элементов
        /// </summary>
        private List<IMediaFactory> InitializeFactories()
        {
            return new List<IMediaFactory>
            {
                new BookFactory(),
                new MovieFactory(),
                new GameFactory(),
                new MusicFactory()
            };
        }

        /// <summary>
        /// Создает новый медиа-элемент указанного типа
        /// </summary>
        public MediaItem CreateMedia(string mediaType)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
                throw new ArgumentException("Тип медиа не может быть пустым", nameof(mediaType));

            var factory = _factories.FirstOrDefault(f => f.GetMediaType() == mediaType);
            return factory?.CreateMediaItem();
        }

        /// <summary>
        /// Добавляет медиа-элемент в каталог и базу данных
        public void AddMedia(MediaItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Медиа-элемент не может быть null");

            if (!item.IsValid())
                throw new InvalidOperationException("Невозможно добавить невалидный медиа-элемент");

            try
            {
                _databaseService.SaveMediaItem(item);
                _mediaCollection.Add(item);
            }
            catch (Exception ex)
            {
                HandleError("Ошибка при добавлении в базу данных", ex);
                throw;
            }
        }

        /// <summary>
        /// Выполняет поиск медиа-элементов по тексту
        /// </summary>
        public List<MediaItem> SearchMedia(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _mediaCollection.ToList();

            return _mediaCollection
                .Where(item =>
                    (item.Title?.ToLower().Contains(searchText.ToLower()) ?? false) ||
                    (item.Genre?.ToLower().Contains(searchText.ToLower()) ?? false) ||
                    (item is Book book && book.Author?.ToLower().Contains(searchText.ToLower()) == true) ||
                    (item is Movie movie && movie.Director?.ToLower().Contains(searchText.ToLower()) == true))
                .ToList();
        }

        /// <summary>
        /// Фильтрует медиа-элементы по типу
        /// </summary>
        public List<MediaItem> FilterByType(string mediaType)
        {
            if (mediaType == "Все")
                return _mediaCollection.ToList();

            return _mediaCollection
                .Where(item => item.GetMediaType() == mediaType)
                .ToList();
        }

        /// <summary>
        /// Изменяет статус медиа-элемента
        /// </summary>
        public void ChangeStatus(MediaItem item, string action)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Медиа-элемент не может быть null");

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Действие не может быть пустым", nameof(action));

            try
            {
                switch (action.ToLower())
                {
                    case "complete":
                        item.Status.MarkCompleted(item);
                        break;
                    case "start":
                        item.Status.MarkInProgress(item);
                        break;
                    case "plan":
                        item.Status.MarkPlanned(item);
                        break;
                    default:
                        throw new ArgumentException($"Неизвестное действие: {action}", nameof(action));
                }

                item.StatusType = item.Status.GetType().Name;
                _databaseService.UpdateMediaItem(item);
            }
            catch (Exception ex)
            {
                HandleError("Ошибка изменения статуса", ex);
                throw;
            }
        }

        /// <summary>
        /// Получает список доступных типов медиа
        /// </summary>
        public List<string> GetAvailableMediaTypes()
        {
            return _factories.Select(f => f.GetMediaType()).ToList();
        }

        /// <summary>
        /// Удаляет медиа-элемент из каталога и базы данных
        /// </summary>
        public void DeleteMedia(MediaItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Медиа-элемент не может быть null");

            try
            {
                _databaseService.DeleteMediaItem(item);
                _mediaCollection.Remove(item);
            }
            catch (Exception ex)
            {
                HandleError("Ошибка удаления", ex);
                throw;
            }
        }

        /// <summary>
        /// Обновляет данные из базы данных
        /// </summary>
        public void RefreshFromDatabase()
        {
            try
            {
                LoadDataFromDatabase();
            }
            catch (Exception ex)
            {
                HandleError("Ошибка обновления данных", ex);
            }
        }

        /// <summary>
        /// Обрабатывает ошибки и показывает сообщение пользователю
        /// </summary>
        private void HandleError(string message, Exception ex)
        {
            string errorMessage = $"{message}: {ex.Message}";
            MessageBox.Show(errorMessage, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}