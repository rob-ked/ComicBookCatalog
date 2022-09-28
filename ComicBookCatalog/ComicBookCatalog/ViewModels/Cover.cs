using ComicBookCatalog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicBookCatalog.ViewModels
{
    class Cover : Base.ViewModel
    {
        public Cover()
        {

        }

        private ComicBook _ComicBook;
        public ComicBook ComicBook
        {
            get
            {
                return _ComicBook;
            }

            set
            {
                if (value != _ComicBook)
                {
                    _ComicBook = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand GoBack => new Command(async () =>
        {
            await Shell.Current.Navigation.PopAsync();
        });

        public override Task OnViewAppearing(object parameter = null)
        {
            if (parameter != null)
            {
                return Task.Run(async () =>
                {
                    ComicBook = await AppShell.DataStore.GetAsync(parameter.ToString());
                });
            }
            else
            {
                throw new Exception("Comic book uuid is missing");
            }
        }
    }
}
