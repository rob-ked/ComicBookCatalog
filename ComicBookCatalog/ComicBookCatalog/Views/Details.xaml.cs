using ComicBookCatalog.Models;
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
    public partial class Details : ContentPage
    {
        public Details()
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

            MessagingCenter.Subscribe<ViewModels.Details>(this, "close", async (sender) =>
            {
                BindingContext = null;
                await this.Navigation.PopAsync();
            });

            MessagingCenter.Subscribe<ViewModels.Edit, ComicBook>(this, "refresh", (editViewModel, updatedComicBook) =>
            {
                ((ViewModels.Details)BindingContext).Refresh(updatedComicBook);
            });

            if (BindingContext != null)
            {
                ((ViewModels.Base.ViewModel)BindingContext).OnViewAppearing(UUID);
            }            
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<ViewModels.Details>(this, "close");
            MessagingCenter.Unsubscribe<ViewModels.Edit>(this, "refresh");
        }
    }
}