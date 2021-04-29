using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MangaScraper.Models
{
    class DatabaseModel : INotifyPropertyChanged
    { 
        private string _server;
        private string _user;
        private string _password;
        private string _database;
        private string _connection;
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
        public string Connection
        {
            get { return _connection; }
            set
            {
                _connection = "server=" + Server + ";user=" + User + ";database=" + Databse + ";password=" + Password;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
