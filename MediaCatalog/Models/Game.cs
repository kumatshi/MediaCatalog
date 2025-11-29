using System;
using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Enums;
using System.Collections.ObjectModel;
using MediaCatalog.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaCatalog.Models
{
    /// <summary>
    /// Класс, представляющий игру в медиа-каталоге
    /// </summary>
    public class Game : MediaItem
    {
        /// <summary>
        /// Платформа для игры
        /// </summary>
        [StringLength(50, ErrorMessage = "Платформа не может превышать 50 символов")]
        public string Platform { get; set; } = string.Empty;

        /// <summary>
        /// Разработчик игры
        /// </summary>
        [StringLength(100, ErrorMessage = "Разработчик не может превышать 100 символов")]
        public string Developer { get; set; } = string.Empty;

        /// <summary>
        /// Время прохождения в часах
        /// </summary>
        [Range(0, 10000, ErrorMessage = "Время игры должно быть от 0 до 10000 часов")]
        public int PlayTime { get; set; }

        /// <summary>
        /// Получает тип медиа-контента
        /// </summary>
        public override string GetMediaType() => "Игра";

        /// <summary>
        /// Проверяет валидность данных игры
        /// </summary>
        public override bool IsValid()
        {
            return base.IsValid() &&
                   !string.IsNullOrWhiteSpace(Platform) &&
                   !string.IsNullOrWhiteSpace(Developer);
        }
    }
}