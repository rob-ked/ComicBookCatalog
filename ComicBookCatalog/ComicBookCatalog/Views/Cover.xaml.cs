using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicBookCatalog.Views
{
    [QueryProperty("UUID", "uuid")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cover : ContentPage
    {
        public Cover()
        {
            InitializeComponent();
        }

        private string _UUID;
        public string UUID
        {
            get
            {
                return _UUID;
            }

            set
            {
                _UUID = Uri.UnescapeDataString(value);
            }

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext != null)
            {
                ((ViewModels.Base.ViewModel)BindingContext).OnViewAppearing(UUID);
            }
        }
    }
}