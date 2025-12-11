using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;
using System.Collections.ObjectModel;
using MediaCatalog.Models;
using MediaCatalog.Patterns.Services;
using Microsoft.Win32;
using System.Diagnostics;

namespace MediaCatalog.Views
{
    public partial class MusicPlayerWindow : Window
    {
        private readonly MediaFacadeService _mediaService;
        private readonly ObservableCollection<MediaItem> _allMediaItems;
        private ObservableCollection<Music> _musicCollection;
        private DispatcherTimer _progressTimer;
        private bool _isUserDraggingSlider = false;

        public MusicPlayerWindow(MediaFacadeService mediaService, ObservableCollection<MediaItem> allMediaItems)
        {
            InitializeComponent();
            _mediaService = mediaService;
            _allMediaItems = allMediaItems;
            _musicCollection = new ObservableCollection<Music>();

            InitializeMusicList();
            SetupProgressTimer();
        }

        private void InitializeMusicList()
        {
            try
            {
                _musicCollection.Clear();
                foreach (var item in _allMediaItems.OfType<Music>())
                {
                    _musicCollection.Add(item);
                }
                MusicListView.ItemsSource = _musicCollection;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки музыки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupProgressTimer()
        {
            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromMilliseconds(500);
            _progressTimer.Tick += ProgressTimer_Tick;
        }

        private void LoadMusicButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Аудио файлы (*.mp3;*.wav;*.flac;*.ogg;*.m4a)|*.mp3;*.wav;*.flac;*.ogg;*.m4a|Все файлы (*.*)|*.*",
                    Title = "Выберите аудио файлы",
                    Multiselect = true
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (var filePath in openFileDialog.FileNames)
                    {
                        AddMusicFileToDatabase(filePath);
                    }

                    InitializeMusicList();
                    MessageBox.Show($"Загружено {openFileDialog.FileNames.Length} файлов",
                        "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки файлов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMusicFileToDatabase(string filePath)
        {
            try
            {
                var fileInfo = new System.IO.FileInfo(filePath);
                var music = new Music
                {
                    Title = System.IO.Path.GetFileNameWithoutExtension(filePath),
                    Artist = "Неизвестный исполнитель",
                    Album = "Неизвестный альбом",
                    Genre = "Неизвестный жанр",
                    Year = DateTime.Now.Year,
                    Rating = 0,
                    Status = new Patterns.States.PlannedState(),
                    StatusType = "PlannedState",
                    FilePath = filePath,
                    FileSize = fileInfo.Length,
                    Format = System.IO.Path.GetExtension(filePath).TrimStart('.').ToLower(),
                    DateAdded = DateTime.UtcNow
                };

                try
                {
                    var tagFile = TagLib.File.Create(filePath);
                    if (!string.IsNullOrWhiteSpace(tagFile.Tag.Title))
                        music.Title = tagFile.Tag.Title;
                    if (tagFile.Tag.Artists != null && tagFile.Tag.Artists.Length > 0)
                        music.Artist = tagFile.Tag.Artists[0];
                    if (!string.IsNullOrWhiteSpace(tagFile.Tag.Album))
                        music.Album = tagFile.Tag.Album;
                    if (tagFile.Tag.Year > 0)
                        music.Year = (int)tagFile.Tag.Year;
                    if (tagFile.Tag.Genres != null && tagFile.Tag.Genres.Length > 0)
                        music.Genre = tagFile.Tag.Genres[0];
                    if (tagFile.Properties.Duration.TotalSeconds > 0)
                        music.Duration = tagFile.Properties.Duration;
                }
                catch { /* Игнорируем ошибки чтения метаданных */ }

                _mediaService.AddMedia(music);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления файла {filePath}: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MusicListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MusicListView.SelectedItem is Music selectedMusic)
            {
                NowPlayingTitle.Text = selectedMusic.Title;
                NowPlayingArtist.Text = selectedMusic.Artist;

                try
                {
                    MediaPlayer.Source = new Uri(selectedMusic.FilePath);
                    MediaPlayer.Play();
                    _progressTimer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка воспроизведения файла: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Source != null)
            {
                MediaPlayer.Play();
                _progressTimer.Start();
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Pause();
            _progressTimer.Stop();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Stop();
            _progressTimer.Stop();
            ProgressSlider.Value = 0;
            UpdateTimeDisplay(TimeSpan.Zero, MediaPlayer.NaturalDuration.TimeSpan);
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            ProgressSlider.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            ProgressSlider.Value = 0;
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _progressTimer.Stop();
            ProgressSlider.Value = 0;

            if (MusicListView.SelectedIndex < MusicListView.Items.Count - 1)
            {
                MusicListView.SelectedIndex++;
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (!_isUserDraggingSlider && MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Value = MediaPlayer.Position.TotalSeconds;
                UpdateTimeDisplay(MediaPlayer.Position, MediaPlayer.NaturalDuration.TimeSpan);
            }
        }

        private void UpdateTimeDisplay(TimeSpan current, TimeSpan total)
        {
            NowPlayingTime.Text = $"{current:mm\\:ss} / {total:mm\\:ss}";
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isUserDraggingSlider && MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                MediaPlayer.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
                UpdateTimeDisplay(MediaPlayer.Position, MediaPlayer.NaturalDuration.TimeSpan);
            }
        }

        private void ProgressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            _isUserDraggingSlider = true;
        }

        private void ProgressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            _isUserDraggingSlider = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            _progressTimer.Stop();
            MediaPlayer.Stop();
            MediaPlayer.Close();
            base.OnClosed(e);
        }
    }
}