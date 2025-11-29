using System;
using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaCatalog.Models
{
    /// <summary>
    /// Класс, представляющий книгу в медиа-каталоге
    /// </summary>
    public class Book : MediaItem
    {
        /// <summary>
        /// Автор книги
        /// </summary>
        [StringLength(100, ErrorMessage = "Имя автора не может превышать 100 символов")]
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Количество страниц
        /// </summary>
        [Range(1, 10000, ErrorMessage = "Количество страниц должно быть от 1 до 10000")]
        public int PageCount { get; set; }

        /// <summary>
        /// ISBN номер книги
        /// </summary>
        [StringLength(20, ErrorMessage = "ISBN не может превышать 20 символов")]
        public string ISBN { get; set; } = string.Empty;

        /// <summary>
        /// Получает тип медиа-контента
        /// </summary>
        /// <returns>Строка "Книга"</returns>
        public override string GetMediaType() => "Книга";

        /// <summary>
        /// Проверяет валидность данных книги
        /// </summary>
        /// <returns>True если данные валидны, иначе False</returns>
        public override bool IsValid()
        {
            return base.IsValid() &&
                   !string.IsNullOrWhiteSpace(Author) &&
                   PageCount > 0;
        }
    }
}