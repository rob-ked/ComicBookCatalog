using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicBookCatalog.ViewModels.Base
{
    abstract class ViewModel : INotifyPropertyChanged
    {
        private bool _IsLoading;
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }

            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLoaded));
                }
            }
        }

        public bool IsLoaded
        {
            get
            {
                return IsLoading == false;
            }
        }

        private string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract Task OnViewAppearing(object parameter = null);                
    }
}
