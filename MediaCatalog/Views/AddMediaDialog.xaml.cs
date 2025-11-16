using MediaCatalog.Models;
using MediaCatalog.Patterns.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MediaCatalog.Views
{
    public partial class AddMediaDialog : Window
    {
        private MediaFacadeService _mediaService;
        private MediaItem _newItem;

        public AddMediaDialog(MediaFacadeService mediaService)
        {
            InitializeComponent();
            _mediaService = mediaService;
            MediaTypeComboBox.ItemsSource = _mediaService.GetAvailableMediaTypes();
            MediaTypeComboBox.SelectedIndex = 0;
        }

        private void MediaTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MediaTypeComboBox.SelectedItem == null) return;

            _newItem = _mediaService.CreateMedia(MediaTypeComboBox.SelectedItem as string);
            DataContext = _newItem;
            UpdateSpecificFields();
        }

        private void UpdateSpecificFields()
        {
            SpecificFieldsPanel.Children.Clear();

            if (_newItem is Book book)
            {
                AddTextField("Автор:", nameof(Book.Author), book.Author);
                AddNumericField("Страниц:", nameof(Book.PageCount), book.PageCount.ToString());
            }
            else if (_newItem is Movie movie)
            {
                AddTextField("Режиссер:", nameof(Movie.Director), movie.Director);
                AddTextField("Длительность (чч:мм):", nameof(Movie.Duration), movie.Duration.ToString());
            }
            else if (_newItem is Game game)
            {
                AddTextField("Платформа:", nameof(Game.Platform), game.Platform);
                AddTextField("Разработчик:", nameof(Game.Developer), game.Developer);
                AddNumericField("Время игры (часы):", nameof(Game.PlayTime), game.PlayTime.ToString());
            }
        }

        private void AddTextField(string label, string propertyName, string initialValue)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            var textBlock = new TextBlock
            {
                Text = label,
                Width = 120,
                VerticalAlignment = VerticalAlignment.Center
            };
            var textBox = new TextBox
            {
                Text = initialValue,
                Width = 200,
                Height = 25,
                Tag = propertyName
            };

            textBox.TextChanged += (s, e) => UpdateProperty(propertyName, textBox.Text);

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            SpecificFieldsPanel.Children.Add(stackPanel);
        }

        private void AddNumericField(string label, string propertyName, string initialValue)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            var textBlock = new TextBlock
            {
                Text = label,
                Width = 120,
                VerticalAlignment = VerticalAlignment.Center
            };
            var textBox = new TextBox
            {
                Text = initialValue,
                Width = 200,
                Height = 25,
                Tag = propertyName
            };

            textBox.TextChanged += (s, e) =>
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    UpdateProperty(propertyName, value);
                }
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            SpecificFieldsPanel.Children.Add(stackPanel);
        }

        private void UpdateProperty(string propertyName, object value)
        {
            if (_newItem == null) return;

            var property = _newItem.GetType().GetProperty(propertyName);
            if (property != null && property.CanWrite)
            {
                try
                {
                    var convertedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(_newItem, convertedValue);
                }
                catch
                {
                    
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_newItem?.Title))
            {
                MessageBox.Show("Введите название медиа", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _mediaService.AddMedia(_newItem);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}