using System;
using System.Collections.Generic;
using System.Text;

namespace MangaScraper
{
    class BaseContent
    {
        /// <summary>
        /// Категория тайтла
        /// </summary>
        public string Category;
        /// <summary>
        /// Название манги
        /// </summary>
        public string Title;
        /// <summary>
        /// Список других названий манги
        /// </summary>
        public List<String> OtherTitles = new List<String>();
        /// <summary>
        /// Фоновое изображение
        /// </summary>
        public string BackgroundImg;
        /// <summary>
        /// Описание манги
        /// </summary>
        public string Description;
        /// <summary>
        /// Количество томов
        /// </summary>
        public byte VolumeNumber;
        /// <summary>
        /// Количество глав
        /// </summary>
        public ushort ChapterNumber;
        /// <summary>
        /// Статус перевода
        /// </summary>
        public string TranslateStatus;
        /// <summary>
        /// Художник
        /// </summary>
        public string Painter;
        /// <summary>
        /// Сценарист
        /// </summary>
        public string Screenwriter;
        /// <summary>
        /// Автор
        /// </summary>
        public string Author;
        /// <summary>
        /// Список глав
        /// </summary>
        public List<String> Chapters = new List<String>();
        /// <summary>
        /// Журнал
        /// </summary>
        public List<String> Magazines = new List<String>();
        /// <summary>
        /// Издатества
        /// </summary>
        public List<String> Publishings = new List<String>();
        /// <summary>
        /// Год выпуска
        /// </summary>
        public ushort ReleaseYear;
        /// <summary>
        /// Список жанров
        /// </summary>
        public List<String> Genres = new List<String>();
        /// <summary>
        /// 
        /// </summary>
        public List<String> Tranlators = new List<String>();
        /// <summary>
        /// Список списков 
        /// </summary>
        public List<List<string>> Images = new List<List<string>>();

    }
}
