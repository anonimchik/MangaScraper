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
        public ObservableCollection<BaseModel> Titles { get; set; }

        public BaseViewModel()
        {
            Scraper sp = new Scraper();
            Titles = new ObservableCollection<BaseModel>();
            sp.Action(Titles);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
