using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraper
{
    class Manhua
    {
        private string Title;
        private byte VolumeNumber;
        private ushort ChapterNumber;
        private List<String> OtherTitles = new List<String>();
        private string ManhuaStatus;
        private string TranslateStatus;
        private List<String> Genres = new List<String>();
        private string Category;
        private List<String> Screenweiters = new List<String>();
        private List<String> Painters = new List<String>();
        private ushort RealeaseYear;
        private List<String> Other = new List<String>();
        private List<String> Publishings = new List<String>();
        private List<String> Tranlators = new List<String>();
    }
}
