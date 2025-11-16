using System;
using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Enums;
using MediaCatalog.Interfaces;

namespace MediaCatalog.Models
{
    public class Movie : MediaItem
    {
        public string Director { get; set; }
        public TimeSpan Duration { get; set; }
        public string Studio { get; set; }

        public override string GetMediaType() => "Фильм";
    }
}