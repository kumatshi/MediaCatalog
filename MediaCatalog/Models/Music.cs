using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaCatalog.Models
{
    /// <summary>
    /// Класс, представляющий музыку в медиа-каталоге
    /// </summary>
    public class Music : MediaItem
    {
        /// <summary>
        /// Исполнитель/группа
        /// </summary>
        [StringLength(100, ErrorMessage = "Исполнитель не может превышать 100 символов")]
        public string Artist { get; set; } = string.Empty;

        /// <summary>
        /// Альбом
        /// </summary>
        [StringLength(100, ErrorMessage = "Альбом не может превышать 100 символов")]
        public string Album { get; set; } = string.Empty;

        /// <summary>
        /// Длительность трека
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Формат файла (mp3, wav, flac и т.д.)
        /// </summary>
        [StringLength(10, ErrorMessage = "Формат не может превышать 10 символов")]
        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// Путь к файлу на диске
        /// </summary>
        [StringLength(500, ErrorMessage = "Путь не может превышать 500 символов")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Размер файла в байтах
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Получает тип медиа-контента
        /// </summary>
        public override string GetMediaType() => "Музыка";

        /// <summary>
        /// Проверяет валидность данных музыки
        /// </summary>
        public override bool IsValid()
        {
            return base.IsValid() &&
                   !string.IsNullOrWhiteSpace(Artist) &&
                   Duration > TimeSpan.Zero &&
                   !string.IsNullOrWhiteSpace(FilePath);
        }
    }
}