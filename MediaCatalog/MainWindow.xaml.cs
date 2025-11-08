using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Enums;
using System.Collections.ObjectModel;

namespace MediaCatalog
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<MediaItem> MediaItems { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MediaItems = new ObservableCollection<MediaItem>();
            MediaListView.ItemsSource = MediaItems;

            LoadSampleData();

            MediaListView.SelectionChanged += (s, e) => ShowItemDetails();
        }

        private void LoadSampleData()
        {
            MediaItems.Add(new Book
            {
                Id = 1,
                Title = "Война и мир",
                Author = "Лев Толстой",
                Year = 1869,
                Genre = "Роман",
                Status = MediaStatus.Completed,
                Rating = 5,
                PageCount = 1225
            });

            MediaItems.Add(new Movie
            {
                Id = 2,
                Title = "Крестный отец",
                Director = "Фрэнсис Форд Коппола",
                Year = 1972,
                Genre = "Криминал, Драма",
                Status = MediaStatus.Planned,
                Rating = 0,
                Duration = new System.TimeSpan(2, 55, 0)
            });

            MediaItems.Add(new Game
            {
                Id = 3,
                Title = "The Witcher 3: Wild Hunt",
                Year = 2015,
                Genre = "RPG",
                Status = MediaStatus.InProgress,
                Rating = 5,
                Platform = "PC",
                Developer = "CD Projekt Red",
                PlayTime = 85
            });
        }

        private void ShowItemDetails()
        {
            if (MediaListView.SelectedItem is MediaItem selectedItem)
            {
                DetailTitle.Text = selectedItem.Title;
                DetailType.Text = selectedItem.GetMediaType();
                DetailYear.Text = selectedItem.Year.ToString();
                DetailGenre.Text = selectedItem.Genre;
                DetailStatus.Text = selectedItem.Status.ToString();
                DetailRating.Text = selectedItem.Rating.ToString() + "/5";

                if (selectedItem is Book book)
                {
                }
                else if (selectedItem is Movie movie)
                {
                }
                else if (selectedItem is Game game)
                {
                }
            }
        }
    }
}