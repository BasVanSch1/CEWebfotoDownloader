using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CEFotoDownloader
{
    class Article : INotifyPropertyChanged, IEquatable<Article>
    {
        public String ArticleCode
        {
            get
            {
                return _articleCode;
            }

            set
            { 
                if (value != _articleCode)
                {
                    _articleCode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public String Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private String _articleCode = "";
        private String _status = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public Article(String articleCode)
        {
            this.ArticleCode = articleCode;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Equals(Article? other)
        {
            if (other != null && other.GetType() == typeof(Article))
            {
                return this.ArticleCode == other.ArticleCode;
            }

            return false;
        }
    }
}
