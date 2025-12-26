using System;
using MediaCatalog.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaCatalog.Models
{
    /// <summary>
    /// Абстрактный базовый класс для всех типов медиа-контента
    /// </summary>
    public abstract class MediaItem
    {
        /// <summary>
        /// Уникальный идентификатор медиа-элемента
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Название медиа-элемента
        /// </summary>
        [Required(ErrorMessage = "Название обязательно для заполнения")]
        [StringLength(200, ErrorMessage = "Название не может превышать 200 символов")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Год выпуска
        /// </summary>
        [Range(1800, 2100, ErrorMessage = "Год должен быть между 1800 и 2100")]
        public int Year { get; set; }

        /// <summary>
        /// Жанр медиа-контента
        /// </summary>
        [StringLength(100, ErrorMessage = "Жанр не может превышать 100 символов")]
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Рейтинг от 1 до 5
        /// </summary>
        [Range(0, 5, ErrorMessage = "Рейтинг должен быть от 0 до 5")]
        public int Rating { get; set; }

        /// <summary>
        /// Текущее состояние медиа-элемента
        /// </summary>
        [NotMapped]
        public IMediaState Status { get; set; } = new Patterns.States.PlannedState();

        /// <summary>
        /// Тип состояния для сохранения в базе данных
        /// </summary>
        public string StatusType { get; set; } = "PlannedState";

        /// <summary>
        /// Дата добавления элемента в каталог
        /// </summary>
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
        public string CoverImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Получает тип медиа-контента
        /// </summary>
        public abstract string GetMediaType();

        /// <summary>
        /// Тип медиа-контента (только для чтения)
        /// </summary>
        [NotMapped]
        public string MediaType => GetMediaType();
        
        /// <summary>
        /// Дата добавления в локальном времени
        /// </summary>
        [NotMapped]
        public DateTime LocalDateAdded => DateAdded.ToLocalTime();

        /// <summary>
        /// Проверяет валидность данных медиа-элемента
        /// </summary>
        public virtual bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Title) &&
                   Year >= 1800 && Year <= DateTime.Now.Year + 5 &&
                   Rating >= 0 && Rating <= 5;
        }
    }
}