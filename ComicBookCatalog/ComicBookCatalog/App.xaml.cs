using Xamarin.Forms;
using ComicBookCatalog.Views;

namespace ComicBookCatalog
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Routing.RegisterRoute("about", typeof(About));
            Routing.RegisterRoute("create", typeof(Edit));
            Routing.RegisterRoute("details", typeof(Details));
            Routing.RegisterRoute("edit", typeof(Edit));
            Routing.RegisterRoute("cover", typeof(Cover));

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
