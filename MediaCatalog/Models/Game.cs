using System;
using System.Windows;
using MediaCatalog.Models;
using MediaCatalog.Enums;
using System.Collections.ObjectModel;
namespace MediaCatalog.Models
{
    public class Game : MediaItem
    {
        public string Platform { get; set; }
        public string Developer { get; set; }
        public int PlayTime { get; set; } // в часах

        public override string GetMediaType() => "Game";
    }
}