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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Catalog : ContentPage
    {
        public Catalog()
        {
            InitializeComponent();            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext != null)
            {
                ((ViewModels.Base.ViewModel)BindingContext).OnViewAppearing();
            }

            MessagingCenter.Subscribe<ViewModels.Edit>(this, "update", (editViewModel) =>
            {
                ((ViewModels.Catalog)BindingContext).UpdateView();
            });

            MessagingCenter.Subscribe<ViewModels.Details>(this, "update", (detailsViewModel) =>
            {
                ((ViewModels.Catalog)BindingContext).UpdateView();
            });

            MessagingCenter.Subscribe<ViewModels.Catalog>(this, "series", (catalogViewModel) =>
            {
                if (this.SeriesPicker.Items.Any()) {
                    this.SeriesPicker.Focus();
                }
            });

            MessagingCenter.Subscribe<ViewModels.Catalog>(this, "brands", (editViewModel) =>
            {
                if (this.BrandsPicker.Items.Any()) { 
                    this.BrandsPicker.Focus();
                }
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // nie usuwamy
            // MessagingCenter.Unsubscribe<ViewModels.Edit>(this, "update");
            // MessagingCenter.Unsubscribe<ViewModels.Details>(this, "update");
            // MessagingCenter.Unsubscribe<ViewModels.Catalog>(this, "series");
            // MessagingCenter.Unsubscribe<ViewModels.Catalog>(this, "brands");
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e != null && BindingContext != null)
            {
                ((ViewModels.Catalog)BindingContext).OpenView.Execute(
                    string.Format("details?uuid={0}", ((ComicBook)e.Item).UUID)
                );
            }
        }
    }
}