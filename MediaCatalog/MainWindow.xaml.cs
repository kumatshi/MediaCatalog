using System;
using System.Windows;
using MediaCatalog.Models;
using System.Collections.ObjectModel;
using MediaCatalog.Patterns.Services;
using System.Windows.Controls;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;

namespace MediaCatalog
{
    /// <summary>
    /// Главное окно приложения Media Catalog
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Коллекция медиа-элементов
        /// </summary>
        public ObservableCollection<MediaItem> MediaItems { get; set; }

        private readonly MediaFacadeService _mediaService;

        /// <summary>
        /// Инициализирует главное окно приложения
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                MediaItems = new ObservableCollection<MediaItem>();

                InitializeUIComponents();
                CheckDatabaseConnection();

                _mediaService = new MediaFacadeService(MediaItems);
                InitializeMediaTypeFilter();

                SubscribeToEvents();

                if (MediaItems.Count > 0)
                {
                    MediaListView.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка инициализации приложения", ex);
            }
        }
      
        /// <summary>
        /// Инициализирует компоненты пользовательского интерфейса
        /// </summary>
        private void InitializeUIComponents()
        {
            if (MediaListView != null)
            {
                MediaListView.ItemsSource = MediaItems;
            }
            else
            {
                throw new InvalidOperationException("MediaListView не найден в XAML");
            }
        }

        /// <summary>
        /// Подписывается на события элементов управления
        /// </summary>
        private void SubscribeToEvents()
        {
            MediaListView.SelectionChanged += MediaListView_SelectionChanged;
            SearchBox.GotFocus += SearchBox_GotFocus;
            SearchBox.LostFocus += SearchBox_LostFocus;
        }

        /// <summary>
        /// Проверяет подключение к базе данных
        /// </summary>
        private void CheckDatabaseConnection()
        {
            try
            {
                using (var context = new Database.MediaCatalogContext())
                {
                    var canConnect = context.Database.CanConnect();
                    if (!canConnect)
                    {
                        ShowWarning("Не удалось подключиться к базе данных. Проверьте настройки подключения.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка подключения к базе данных", ex);
            }
        }

        /// <summary>
        /// Обрабатывает изменение выбранного элемента в списке
        /// </summary>
        private void MediaListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (MediaListView.SelectedItem is MediaItem selectedItem)
                {
                    ShowItemDetails(selectedItem);
                }
                else
                {
                    ClearItemDetails();
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка отображения деталей", ex);
            }
        }

        /// <summary>
        /// Показывает детальную информацию о выбранном элементе
        /// </summary>
        private void ShowItemDetails(MediaItem item)
        {
            if (item == null) return;

            try
            {
                DetailTitle.Text = item.Title ?? "Не указано";
                DetailType.Text = item.MediaType ?? "Не указано";
                DetailYear.Text = item.Year.ToString();
                DetailGenre.Text = item.Genre ?? "Не указано";
                DetailStatus.Text = item.Status?.Name ?? "Не указано";
                DetailRating.Text = item.Rating.ToString();
                DetailDateAdded.Text = item.LocalDateAdded.ToString("dd.MM.yyyy HH:mm");

                ClearSpecificDetails();
                AddSpecificDetails(item);
            }
            catch (Exception ex)
            {
                ShowError("Ошибка отображения деталей элемента", ex);
            }
        }

        /// <summary>
        /// Добавляет специфические детали в зависимости от типа медиа
        /// </summary>
        private void AddSpecificDetails(MediaItem item)
        {
            if (item is Book book)
            {
                AddDetailField("Автор:", book.Author ?? "Не указано");
                AddDetailField("Количество страниц:", book.PageCount.ToString());
                AddDetailField("ISBN:", book.ISBN ?? "Не указано");
            }
            else if (item is Movie movie)
            {
                AddDetailField("Режиссер:", movie.Director ?? "Не указано");
                AddDetailField("Длительность:", movie.Duration.ToString(@"hh\:mm"));
                AddDetailField("Студия:", movie.Studio ?? "Не указано");
            }
            else if (item is Game game)
            {
                AddDetailField("Платформа:", game.Platform ?? "Не указано");
                AddDetailField("Разработчик:", game.Developer ?? "Не указано");
                AddDetailField("Время игры (часы):", game.PlayTime.ToString());
            }
            else if (item is Music music) 
            {
                AddDetailField("Исполнитель:", music.Artist ?? "Не указано");
                AddDetailField("Альбом:", music.Album ?? "Не указано");
                AddDetailField("Длительность:", music.Duration.ToString(@"hh\:mm\:ss"));
                AddDetailField("Формат:", music.Format ?? "Не указано");
                AddDetailField("Размер файла:", FormatFileSize(music.FileSize));
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

    

        /// <summary>
        /// Очищает детальную информацию
        /// </summary>
        private void ClearItemDetails()
        {
            DetailTitle.Text = "";
            DetailType.Text = "";
            DetailYear.Text = "";
            DetailGenre.Text = "";
            DetailStatus.Text = "";
            DetailRating.Text = "";
            DetailDateAdded.Text = "";
            ClearSpecificDetails();
        }

        /// <summary>
        /// Очищает специфические поля деталей
        /// </summary>
        private void ClearSpecificDetails()
        {
            var specificFields = DetailPanel.Children
                .OfType<StackPanel>()
                .Where(sp => sp.Tag?.ToString() == "SpecificField")
                .ToList();

            foreach (var field in specificFields)
            {
                DetailPanel.Children.Remove(field);
            }
        }

        /// <summary>
        /// Добавляет поле детальной информации
        /// </summary>
        private void AddDetailField(string label, string value)
        {
            var stackPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 10),
                Tag = "SpecificField"
            };

            var labelText = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 2)
            };

            var valueText = new TextBlock
            {
                Text = value,
                TextWrapping = TextWrapping.Wrap
            };

            stackPanel.Children.Add(labelText);
            stackPanel.Children.Add(valueText);

            var insertIndex = DetailPanel.Children.Count - 1;
            DetailPanel.Children.Insert(insertIndex, stackPanel);
        }

        /// <summary>
        /// Инициализирует фильтр по типам медиа
        /// </summary>
        private void InitializeMediaTypeFilter()
        {
            if (FilterComboBox == null) return;

            try
            {
                FilterComboBox.Items.Clear();
                FilterComboBox.Items.Add("Все");
                foreach (var type in _mediaService.GetAvailableMediaTypes())
                {
                    FilterComboBox.Items.Add(type);
                }
                FilterComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowError("Ошибка инициализации фильтра", ex);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки добавления
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Views.AddMediaDialog(_mediaService);
                if (dialog.ShowDialog() == true)
                {
                    MediaListView.Items.Refresh();

                    if (MediaItems.Count > 0)
                    {
                        MediaListView.SelectedItem = MediaItems[^1];
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка добавления медиа", ex);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки завершения
        /// </summary>
        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MediaListView?.SelectedItem is MediaItem selectedItem)
                {
                    _mediaService.ChangeStatus(selectedItem, "Complete");
                    MediaListView.Items.Refresh();
                    ShowItemDetails(selectedItem);
                    ShowInfo("Статус изменен на 'Завершено'");
                }
                else
                {
                    ShowWarning("Выберите элемент для изменения статуса");
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка изменения статуса", ex);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки удаления
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MediaListView?.SelectedItem is MediaItem selectedItem)
                {
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить '{selectedItem.Title}'?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        int selectedIndex = MediaListView.SelectedIndex;
                        _mediaService.DeleteMedia(selectedItem);

                        if (MediaItems.Count > 0)
                        {
                            MediaListView.SelectedIndex = Math.Min(selectedIndex, MediaItems.Count - 1);
                        }
                        else
                        {
                            ClearItemDetails();
                        }

                        ShowInfo("Элемент успешно удален");
                    }
                }
                else
                {
                    ShowWarning("Выберите элемент для удаления");
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка удаления", ex);
            }
        }

        /// <summary>
        /// Обрабатывает изменение текста поиска
        /// </summary>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (MediaListView == null) return;

                var searchText = SearchBox.Text;
                if (string.IsNullOrWhiteSpace(searchText) || searchText == "Поиск...")
                {
                    MediaListView.ItemsSource = MediaItems;
                }
                else
                {
                    var results = _mediaService.SearchMedia(searchText);
                    MediaListView.ItemsSource = results;

                    if (results.Count == 0)
                    {
                        ClearItemDetails();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка поиска", ex);
            }
        }

        /// <summary>
        /// Обрабатывает изменение фильтра
        /// </summary>
        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (MediaListView == null) return;

                var selectedType = FilterComboBox.SelectedItem as string;
                if (selectedType == "Все")
                {
                    MediaListView.ItemsSource = MediaItems;
                }
                else
                {
                    var results = _mediaService.FilterByType(selectedType);
                    MediaListView.ItemsSource = results;

                    if (results.Count == 0)
                    {
                        ClearItemDetails();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка фильтрации", ex);
            }
        }

        /// <summary>
        /// Обрабатывает получение фокуса поисковой строкой
        /// </summary>
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Поиск...")
            {
                SearchBox.Text = "";
            }
        }

        /// <summary>
        /// Обрабатывает потерю фокуса поисковой строкой
        /// </summary>
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Поиск...";
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки "В процессе"
        /// </summary>
        private void StartProgressButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MediaListView?.SelectedItem is MediaItem selectedItem)
                {
                    _mediaService.ChangeStatus(selectedItem, "Start");
                    MediaListView.Items.Refresh();
                    ShowItemDetails(selectedItem);
                    ShowInfo("Статус изменен на 'В процессе'");
                }
                else
                {
                    ShowWarning("Выберите элемент для изменения статуса");
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка изменения статуса", ex);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки "В планы"
        /// </summary>
        private void PlanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MediaListView?.SelectedItem is MediaItem selectedItem)
                {
                    _mediaService.ChangeStatus(selectedItem, "Plan");
                    MediaListView.Items.Refresh();
                    ShowItemDetails(selectedItem);
                    ShowInfo("Статус изменен на 'В планах'");
                }
                else
                {
                    ShowWarning("Выберите элемент для изменения статуса");
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка изменения статуса", ex);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки обновления
        /// </summary>
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mediaService.RefreshFromDatabase();
                MediaListView.Items.Refresh();
                ShowInfo("Данные обновлены из базы данных");
            }
            catch (Exception ex)
            {
                ShowError("Ошибка обновления", ex);
            }
        }

        /// <summary>
        /// Показывает сообщение об ошибке
        /// </summary>
        private void ShowError(string message, Exception ex)
        {
            string errorMessage = $"{message}: {ex.Message}";
            MessageBox.Show(errorMessage, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Показывает предупреждение
        /// </summary>
        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Предупреждение",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Показывает информационное сообщение
        /// </summary>
        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Обрабатывает закрытие окна
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при закрытии приложения", ex);
            }
        }
        private void MusicButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var musicWindow = new Views.MusicPlayerWindow(_mediaService, MediaItems);
                musicWindow.Owner = this;
                musicWindow.Show();
            }
            catch (Exception ex)
            {
                ShowError("Ошибка открытия музыкального плеера", ex);
            }
        }
    }
}