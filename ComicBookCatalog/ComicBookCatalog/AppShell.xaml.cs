using ComicBookCatalog.Models;
using ComicBookCatalog.Services;

namespace ComicBookCatalog
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public static IDataStore<ComicBook> DataStore { get; set; }

        public AppShell()
        {
            InitializeComponent();
        }
    }
}
