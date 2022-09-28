using ComicBookCatalog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicBookCatalog.ViewModels
{
    class About : Base.ViewModel
    {
        public About()
        {

        }

        public ApplicationInfo ApplicationInfo
        {
            get
            {
                return new ApplicationInfo();
            }
        }

        public override Task OnViewAppearing(object parameter = null)
        {
            return null;
        }

        public ICommand GoBack => new Command(async () =>
        {
            await Shell.Current.GoToAsync("..");            
        });
    }
}
