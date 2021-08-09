using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MangaScraper
{
    class BaseModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Ссылка на источник
        /// </summary>
        private string _sourceUrl;
        /// <summary>
        /// Категория тайтла
        /// </summary>
        private string _category;
        /// <summary>
        /// Название манги
        /// </summary>
        private string _title;
        /// <summary>
        /// Английское название
        /// </summary>
        private string _engTitle;
        /// <summary>
        /// Альтернативные названия
        /// </summary>
        private List<String> _otherTitles = new List<String>();
        /// <summary>
        /// Фоновое изображение
        /// </summary>
        private string _backgroundImg;
        /// <summary>
        /// Описание манги
        /// </summary>
        private string _description;
        /// <summary>
        /// Количество томов
        /// </summary>
        private byte _volumeNumber;
        /// <summary>
        /// Количество глав
        /// </summary>
        private ushort _chapterNumber;
        /// <summary>
        /// Статус перевода
        /// </summary>
        private string _TranslateStatus;
        /// <summary>
        /// Статус тайтла
        /// </summary>
        private string _titleStatus;
        /// <summary>
        /// Художник
        /// </summary>
        private List<String> _painters = new List<String>();
        /// <summary>
        /// Сценарист
        /// </summary>
        private List<String> _screenwriters = new List<String>();
        /// <summary>
        /// Автор
        /// </summary>
        private string _author;
        /// <summary>
        /// Список авторов
        /// </summary>
        private List<String> _authors = new List<String>();
        /// <summary>
        /// Список глав
        /// </summary>
        private List<String> _chapters = new List<String>();
        /// <summary>
        /// Журнал
        /// </summary>
        private List<String> _magazines = new List<String>();
        /// <summary>
        /// Издатества
        /// </summary>
        private List<String> _publishings = new List<String>();
        /// <summary>
        /// Год выпуска
        /// </summary>
        private ushort _releaseYear;
        /// <summary>
        /// Список жанров
        /// </summary>
        private List<String> _genres = new List<String>();
        /// <summary>
        /// Переводчики
        /// </summary>
        private List<String> _tranlators = new List<String>();
        /// <summary>
        /// Список расширений в конкретной главе
        /// </summary>
        private List<List<String>> _expansion = new List<List<String>>();
        /// <summary>
        /// Список списков 
        /// </summary>
        private List<List<String>> _images = new List<List<String>>();
        /// <summary>
        /// Путь к картинке в локальном хранилище
        /// </summary>
        private List<List<String>> _pathImages = new List<List<String>>();
        public string SourceUrl
        {
            get { return _sourceUrl; }
            set
            {
                _sourceUrl = value;
                OnPropertyChanged("SourceUrl");
            }
        }
        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged("Category");
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string EngTitle
        {
            get { return _engTitle; }
            set
            {
                _engTitle = value;
                OnPropertyChanged("EngTitle");
            }
        }

        public List<String> OtherTitles
        {
            get { return _otherTitles; }
            set
            { _otherTitles = value;
                OnPropertyChanged("OtherTitles");
            }
        }

        public string BackgroundImg
        {
            get { return _backgroundImg; }
            set
            {
                _backgroundImg = value;
                OnPropertyChanged("BackgroundImg");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public byte VolumeNumber
        {
            get { return _volumeNumber; }
            set
            {
                _volumeNumber = value;
                OnPropertyChanged("VolumeNumber");
            }
        }

        public ushort ChapterNumber
        {
            get { return _chapterNumber; }
            set
            {
                _chapterNumber = value;
                OnPropertyChanged("ChapterNumber");
            }
        }

        public string TranslateStatus
        {
            get { return _TranslateStatus; }
            set
            {
                _TranslateStatus = value;
                OnPropertyChanged("TranslateStatus");
            }
        }

        public string TitleStatus
        {
            get { return _titleStatus; }
            set
            {
                _titleStatus = value;
                OnPropertyChanged("TitleStatus");
            }
        }

        public List<String> Painters
        {
            get { return _painters; }
            set
            {
                _painters = value;
                OnPropertyChanged("Painter");
            }
        }

        public List<String> Screenwriters
        {
            get { return _screenwriters; }
            set
            {
                _screenwriters = value;
                OnPropertyChanged("Screenwriters");
            }
        }

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged("Author");
            }
        }

        public List<String> Authors
        {
            get { return _authors; }
            set
            {
                _authors = value;
                OnPropertyChanged("Authors");
            }
        }

        public List<String> Chapters
        {
            get { return _chapters; }
            set
            {
                _chapters = value;
                OnPropertyChanged("Chapters");
            }
        }

        public List<String> Magazines
        {
            get { return _magazines; }
            set
            {
                _magazines = value;
                OnPropertyChanged("Magazines");
            }
        }

        public List<String> Publishings
        {
            get { return _publishings; }
            set
            {
                _publishings = value;
                OnPropertyChanged("Publishings");
            }
        }

        public ushort ReleaseYear
        {
            get { return _releaseYear; }
            set
            {
                _releaseYear = value;
                OnPropertyChanged("ReleaseYear");
            }
        }

        public List<String> Genres
        {
            get { return _genres; }
            set
            {
                _genres = value;
                OnPropertyChanged("Genres");
            }
        }

        public List<String> Translators
        {
            get { return _tranlators; }
            set
            {
                _tranlators = value;
                OnPropertyChanged("Translators");
            }
        }

        public List<List<String>> Expansion
        {
            get { return _expansion; }
            set
            {
                _expansion = value;
                OnPropertyChanged("Expansion");
            }
        }
        public List<List<String>> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                OnPropertyChanged("Images");
            }
        }

        public List<List<String>> PathImages
        {
            get { return _pathImages; }
            set
            {
                _pathImages = value;
                OnPropertyChanged("PathImages");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
