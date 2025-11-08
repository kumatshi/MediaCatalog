using System;
using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Enums;
using System.Collections.ObjectModel;

namespace MediaCatalog.Models
{
    public abstract class MediaItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public int Rating { get; set; }
        public MediaStatus Status { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public abstract string GetMediaType();
    }
}