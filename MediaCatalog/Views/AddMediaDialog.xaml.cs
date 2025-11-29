using MediaCatalog.Models;
using MediaCatalog.Patterns.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace MediaCatalog.Views
{
    /// <summary>
    /// Диалоговое окно для добавления нового медиа-элемента
    /// </summary>
    public partial class AddMediaDialog : Window, INotifyPropertyChanged
    {
        private readonly MediaFacadeService _mediaService;
        private MediaItem _newItem;
        private string _validationMessage = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Сообщение о валидации
        /// </summary>
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValidationMessage)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
            }
        }

        /// <summary>
        /// Показывает, валидны ли данные
        /// </summary>
        public bool IsValid => string.IsNullOrEmpty(ValidationMessage);

        /// <summary>
        /// Инициализирует новый экземпляр диалогового окна
        /// </summary>
        public AddMediaDialog(MediaFacadeService mediaService)
        {
            InitializeComponent();
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));

            InitializeMediaTypes();
            DataContext = this;
        }

        /// <summary>
        /// Инициализирует список типов медиа
        /// </summary>
        private void InitializeMediaTypes()
        {
            try
            {
                MediaTypeComboBox.ItemsSource = _mediaService.GetAvailableMediaTypes();
                MediaTypeComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ShowError("Ошибка загрузки типов медиа", ex);
            }
        }

        /// <summary>
        /// Обрабатывает изменение выбранного типа медиа
        /// </summary>
        private void MediaTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (MediaTypeComboBox.SelectedItem == null) return;

                _newItem = _mediaService.CreateMedia(MediaTypeComboBox.SelectedItem as string);
                DataContext = _newItem;
                TitleTextBox.TextChanged += (s, args) => ValidateData();
                YearTextBox.TextChanged += (s, args) => ValidateData();
                GenreTextBox.TextChanged += (s, args) => ValidateData();
                RatingTextBox.TextChanged += (s, args) => ValidateData();
                UpdateSpecificFields();
                ValidateData();
            }
            catch (Exception ex)
            {
                ShowError("Ошибка создания медиа-элемента", ex);
            }
        }

        /// <summary>
        /// Обновляет специфические поля для выбранного типа медиа
        /// </summary>
        private void UpdateSpecificFields()
        {
            try
            {
                SpecificFieldsPanel.Children.Clear();

                if (_newItem is Book book)
                {
                    AddTextField("Автор:", nameof(Book.Author), book.Author, true);
                    AddNumericField("Страниц:", nameof(Book.PageCount), book.PageCount.ToString(), true);
                    AddTextField("ISBN:", nameof(Book.ISBN), book.ISBN, false);
                }
                else if (_newItem is Movie movie)
                {
                    AddTextField("Режиссер:", nameof(Movie.Director), movie.Director, true);
                    AddDurationField("Длительность (чч:мм):", nameof(Movie.Duration), movie.Duration.ToString(@"hh\:mm"), true);
                    AddTextField("Студия:", nameof(Movie.Studio), movie.Studio, false);
                }
                else if (_newItem is Game game)
                {
                    AddTextField("Платформа:", nameof(Game.Platform), game.Platform, true);
                    AddTextField("Разработчик:", nameof(Game.Developer), game.Developer, true);
                    AddNumericField("Время игры (часы):", nameof(Game.PlayTime), game.PlayTime.ToString(), false);
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка обновления полей", ex);
            }
        }

        /// <summary>
        /// Добавляет текстовое поле в форму
        /// </summary>
        private void AddTextField(string label, string propertyName, string initialValue, bool isRequired)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            var textBlock = new TextBlock
            {
                Text = label + (isRequired ? " *" : ""),
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
                UpdateProperty(propertyName, textBox.Text);
                ValidateData();
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            SpecificFieldsPanel.Children.Add(stackPanel);
        }

        /// <summary>
        /// Добавляет числовое поле в форму
        /// </summary>
        private void AddNumericField(string label, string propertyName, string initialValue, bool isRequired)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            var textBlock = new TextBlock
            {
                Text = label + (isRequired ? " *" : ""),
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
                    ValidateData();
                }
                else if (string.IsNullOrEmpty(textBox.Text))
                {
                    UpdateProperty(propertyName, 0);
                    ValidateData();
                }
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            SpecificFieldsPanel.Children.Add(stackPanel);
        }

        /// <summary>
        /// Добавляет поле для ввода времени в форму
        /// </summary>
        private void AddDurationField(string label, string propertyName, string initialValue, bool isRequired)
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

            var textBlock = new TextBlock
            {
                Text = label + (isRequired ? " *" : ""),
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
                if (TimeSpan.TryParse(textBox.Text, out TimeSpan value))
                {
                    UpdateProperty(propertyName, value);
                    ValidateData();
                }
            };

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBox);
            SpecificFieldsPanel.Children.Add(stackPanel);
        }

        /// <summary>
        /// Обновляет свойство медиа-элемента
        /// </summary>
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

        /// <summary>
        /// Валидирует данные формы
        /// </summary>
        private void ValidateData()
        {
            if (_newItem == null)
            {
                ValidationMessage = "Медиа-элемент не создан";
                return;
            }

            if (string.IsNullOrWhiteSpace(_newItem.Title) || _newItem.Title == "Новый элемент")
            {
                ValidationMessage = "Введите название медиа";
                return;
            }

            if (_newItem.Year < 1800 || _newItem.Year > DateTime.Now.Year + 5)
            {
                ValidationMessage = "Введите корректный год (от 1800 до текущего года + 5)";
                return;
            }

            if (_newItem.Rating < 0 || _newItem.Rating > 5)
            {
                ValidationMessage = "Рейтинг должен быть от 0 до 5";
                return;
            }

            if (!_newItem.IsValid())
            {
                ValidationMessage = "Заполните все обязательные поля корректными данными";
                return;
            }

            ValidationMessage = string.Empty;
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки сохранения
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateData();

                if (!IsValid)
                {
                    MessageBox.Show(ValidationMessage, "Ошибка валидации",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_newItem?.Title) || _newItem.Title == "Новый элемент")
                {
                    MessageBox.Show("Введите название медиа", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_newItem.Year < 1800 || _newItem.Year > DateTime.Now.Year + 5)
                {
                    MessageBox.Show("Введите корректный год", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_newItem.Rating < 0 || _newItem.Rating > 5)
                {
                    MessageBox.Show("Рейтинг должен быть от 0 до 5", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _newItem.DateAdded = DateTime.UtcNow;
                _mediaService.AddMedia(_newItem);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowError("Ошибка при сохранении", ex);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие кнопки отмены
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
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
        /// Вызывает событие PropertyChanged
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}