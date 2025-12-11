using MediaCatalog.Database;
using MediaCatalog.Interfaces;
using MediaCatalog.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MediaCatalog.Services
{
    /// <summary>
    /// Сервис для работы с базой данных медиа-каталога
    /// </summary>
    public class DatabaseService
    {
        private readonly MediaCatalogContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр DatabaseService
        /// </summary>
        public DatabaseService()
        {
            _context = new MediaCatalogContext();
        }

        /// <summary>
        /// Инициализирует базу данных и создает таблицы при необходимости
        /// </summary>
        public void InitializeDatabase()
        {
            try
            {
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                _context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                HandleError("Ошибка инициализации базы данных", ex);
            }
        }

        /// <summary>
        /// Загружает все медиа-элементы из базы данных
        /// </summary>
        public ObservableCollection<MediaItem> LoadMediaItems()
        {
            var allItems = new ObservableCollection<MediaItem>();

            try
            {
                var books = _context.Books.ToList();
                var movies = _context.Movies.ToList();
                var games = _context.Games.ToList();
                var musics = _context.Musics.ToList();

                foreach (var book in books)
                {
                    book.Status = CreateStateFromString(book.StatusType);
                    allItems.Add(book);
                }

                foreach (var movie in movies)
                {
                    movie.Status = CreateStateFromString(movie.StatusType);
                    allItems.Add(movie);
                }

                foreach (var game in games)
                {
                    game.Status = CreateStateFromString(game.StatusType);
                    allItems.Add(game);
                }
                foreach (var music in musics) // Добавить этот блок
                {
                    music.Status = CreateStateFromString(music.StatusType);
                    allItems.Add(music);
                }
            }
            catch (Exception ex)
            {
                HandleError("Ошибка загрузки данных", ex);
            }

            return allItems;
        }

        /// <summary>
        /// Сохраняет медиа-элемент в базу данных
        /// </summary>
        public void SaveMediaItem(MediaItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Медиа-элемент не может быть null");

            if (!item.IsValid())
                throw new InvalidOperationException("Данные медиа-элемента невалидны");

            try
            {
                item.StatusType = item.Status?.GetType().Name ?? "PlannedState";

                if (item.DateAdded.Kind != DateTimeKind.Utc)
                {
                    item.DateAdded = item.DateAdded.ToUniversalTime();
                }

                if (item is Book book)
                {
                    if (book.Id == 0)
                        _context.Books.Add(book);
                    else
                        _context.Books.Update(book);
                }
                else if (item is Movie movie)
                {
                    if (movie.Id == 0)
                        _context.Movies.Add(movie);
                    else
                        _context.Movies.Update(movie);
                }
                else if (item is Game game)
                {
                    if (game.Id == 0)
                        _context.Games.Add(game);
                    else
                        _context.Games.Update(game);
                }
                else if (item is Music music) // ДОБАВЬТЕ ЭТОТ БЛОК
                {
                    if (music.Id == 0)
                        _context.Musics.Add(music);
                    else
                        _context.Musics.Update(music);
                }
                else
                {
                    throw new InvalidOperationException($"Неизвестный тип медиа: {item.GetType().Name}");
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleError("Ошибка сохранения", ex);
                throw;
            }
        }

        /// <summary>
        /// Удаляет медиа-элемент из базы данных
        /// </summary>
        public void DeleteMediaItem(MediaItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Медиа-элемент не может быть null");

            try
            {
                if (item is Book book)
                    _context.Books.Remove(book);
                else if (item is Movie movie)
                    _context.Movies.Remove(movie);
                else if (item is Game game)
                    _context.Games.Remove(game);
                else if (item is Music music) // ДОБАВЬТЕ
                    _context.Musics.Remove(music);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleError("Ошибка удаления", ex);
                throw;
            }
        }

        /// <summary>
        /// Обновляет медиа-элемент в базе данных
        /// </summary>
        public void UpdateMediaItem(MediaItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Медиа-элемент не может быть null");

            try
            {
                item.StatusType = item.Status?.GetType().Name ?? "PlannedState";

                if (item.DateAdded.Kind != DateTimeKind.Utc)
                {
                    item.DateAdded = item.DateAdded.ToUniversalTime();
                }

                if (item is Book book)
                    _context.Books.Update(book);
                else if (item is Movie movie)
                    _context.Movies.Update(movie);
                else if (item is Game game)
                    _context.Games.Update(game);
                else if (item is Music music) // ДОБАВЬТЕ
                    _context.Musics.Update(music);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleError("Ошибка обновления", ex);
                throw;
            }
        }

        /// <summary>
        /// Создает объект состояния из строкового представления
        /// </summary>
        private static IMediaState CreateStateFromString(string stateName)
        {
            return stateName switch
            {
                "PlannedState" => new Patterns.States.PlannedState(),
                "InProgressState" => new Patterns.States.InProgressState(),
                "CompletedState" => new Patterns.States.CompletedState(),
                _ => new Patterns.States.PlannedState()
            };
        }

        /// <summary>
        /// Обрабатывает ошибки и показывает сообщение пользователю
        /// </summary>
        private void HandleError(string message, Exception ex)
        {
            string errorMessage = $"{message}: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\n\nДетали: {ex.InnerException.Message}";
            }

            MessageBox.Show(errorMessage, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}