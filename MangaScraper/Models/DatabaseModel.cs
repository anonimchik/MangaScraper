using System.ComponentModel;

namespace MangaScraper.Models
{
    class DatabaseModel : INotifyPropertyChanged
    { 
        private string _server;
        private string _user;
        private string _password;
        private string _database;
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string Databse
        {
            get { return _database; }
            set { _database = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
