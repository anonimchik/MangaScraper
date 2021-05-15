using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

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
            /*
            ReadMangaViewModel sp = new ReadMangaViewModel();
            Titles = new ObservableCollection<BaseModel>();
            sp.parseInfo(Titles);
            */
            MangachanViewModel mc = new MangachanViewModel();
            mc.MangachanMain();
            
            //sp.writeToFile(Titles);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
}
