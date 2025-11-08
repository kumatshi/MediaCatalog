using System;
using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Enums;
using System.Collections.ObjectModel;

namespace MediaCatalog.Models
{
    public class Book : MediaItem
    {
        public string Author { get; set; }
        public int PageCount { get; set; }
        public string ISBN { get; set; }

        public override string GetMediaType() => "Book";
    }
}