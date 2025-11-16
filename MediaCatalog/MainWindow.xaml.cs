using System.Windows;
using MediaCatalog.Models;
using System.Collections.ObjectModel;
using MediaCatalog.Patterns.Services;
using System.Windows.Controls;
using System.Linq;

namespace MediaCatalog
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<MediaItem> MediaItems { get; set; }
        private MediaFacadeService _mediaService;

        public MainWindow()
        {
            InitializeComponent();
            MediaItems = new ObservableCollection<MediaItem>();
            if (MediaListView != null)
            {
                MediaListView.ItemsSource = MediaItems;
            }
            else
            {
                MessageBox.Show("MediaListView не найден в XAML");
                return;
            }

            _mediaService = new MediaFacadeService(MediaItems);

            InitializeMediaTypeFilter();
            LoadSampleData();
        }

        private void InitializeMediaTypeFilter()
        {
            if (FilterComboBox == null) return;

            FilterComboBox.Items.Clear();
            FilterComboBox.Items.Add("Все");
            foreach (var type in _mediaService.GetAvailableMediaTypes())
            {
                FilterComboBox.Items.Add(type);
            }
            FilterComboBox.SelectedIndex = 0;
        }

        private void LoadSampleData()
        {
            try
            {
                var book = _mediaService.CreateMedia("Книга");
                book.Title = "Война и мир";
                book.Year = 1869;
                book.Genre = "Роман";
                book.Rating = 5;
                ((Book)book).Author = "Лев Толстой";
                ((Book)book).PageCount = 1225;
                _mediaService.AddMedia(book);

                var movie = _mediaService.CreateMedia("Фильм");
                movie.Title = "Крестный отец";
                movie.Year = 1972;
                movie.Genre = "Криминал, Драма";
                movie.Rating = 5;
                ((Movie)movie).Director = "Фрэнсис Форд Коппола";
                ((Movie)movie).Duration = new System.TimeSpan(2, 55, 0);
                _mediaService.AddMedia(movie);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Views.AddMediaDialog(_mediaService);
            if (dialog.ShowDialog() == true)
            {
                if (MediaListView != null)
                {
                    MediaListView.Items.Refresh();
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
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
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            }
        }

        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaListView?.SelectedItem is MediaItem selectedItem)
            {
                _mediaService.ChangeStatus(selectedItem, "Complete");
                MediaListView.Items.Refresh();
            }
        }
    }
}