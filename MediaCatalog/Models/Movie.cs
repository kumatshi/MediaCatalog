using System;
using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Enums;
using MediaCatalog.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaCatalog.Models
{
    /// <summary>
    /// Класс, представляющий фильм в медиа-каталоге
    /// </summary>
    public class Movie : MediaItem
    {
        /// <summary>
        /// Режиссер фильма
        /// </summary>
        [StringLength(100, ErrorMessage = "Имя режиссера не может превышать 100 символов")]
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Продолжительность фильма
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Студия производства
        /// </summary>
        [StringLength(100, ErrorMessage = "Название студии не может превышать 100 символов")]
        public string Studio { get; set; } = string.Empty;

        /// <summary>
        /// Получает тип медиа-контента
        /// </summary>
        public override string GetMediaType() => "Фильм";

        /// <summary>
        /// Проверяет валидность данных фильма
        /// </summary>
        public override bool IsValid()
        {
            return base.IsValid() &&
                   !string.IsNullOrWhiteSpace(Director) &&
                   Duration > TimeSpan.Zero;
        }
    }
}