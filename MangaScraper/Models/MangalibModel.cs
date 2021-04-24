using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MangaScraper.Models
{
    class MangalibModel : INotifyPropertyChanged
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
