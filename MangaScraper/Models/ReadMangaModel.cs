using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.ComponentModel;

namespace MangaScraper
{
    class ReadMangaModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Ссылка на сайт
        /// </summary>
        private string _url;
        /// <summary>
        /// Списоск ссылок на тайтлы
        /// </summary>
        private List<String> _titleUrl;
        /// <summary>
        /// Ссылка на следующую страницу
        /// </summary>
        private string _nextPageUrl;
        /// <summary>
        /// Список ссылок на тайтлы
        /// </summary>
        private List<String> _mangaUrl;
        /// <summary>
        /// Счетчик ошибок
        /// </summary>
        private ushort _failCounter;
        /// <summary>
        /// Сообщение о ошибке
        /// </summary>
        private string _errorMessage;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }
        public List<String> TitleUrl
        {
            get { return _titleUrl; }
            set { _titleUrl = value; }
        }
        public List<String> MangaUrl
        {
            get { return _mangaUrl; }
            set { _mangaUrl = value; }
        }
        public string NextPageUrl
        {
            get { return _nextPageUrl; }
            set { _nextPageUrl = value; }
        }
        public ushort FailCounter
        {
            get { return _failCounter; }
            set { _failCounter = value; }
        }
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
