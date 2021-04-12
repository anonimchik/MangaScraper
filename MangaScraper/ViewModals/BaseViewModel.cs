using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.ObjectModel;


namespace MangaScraper.ViewModals
{
    class BaseViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<BaseModel> _titles;
        public ObservableCollection<BaseModel> Titles
        {
            get { return _titles; }
            set
            {
                _titles = value;
                OnPropertyChanged();
            }
        }

        public BaseViewModel()
        {
            Scraper sp = new Scraper();
            Titles = new ObservableCollection<BaseModel>();
            sp.parseInfo(Titles);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
