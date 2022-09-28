using ComicBookCatalog.Enums;
using ComicBookCatalog.Models;
using ComicBookCatalog.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicBookCatalog.ViewModels
{
    class Catalog : Base.ViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        private ListContent currentContent = ListContent.ANY;

        /// <summary>
        /// 
        /// </summary>
        public Catalog()
        {
            
        }

        private ObservableCollection<Grouping<string, ComicBook>> _ComicBooks = new ObservableCollection<Grouping<string, ComicBook>>();
        public ObservableCollection<Grouping<string, ComicBook>> ComicBooks
        {
            get
            {
                return _ComicBooks;
            }

            set
            {
                if (_ComicBooks != value)
                {
                    _ComicBooks = value;
                    OnPropertyChanged();                    
                }
            }
        }

        private bool _IsComicBooksListRefreshing;
        public bool IsComicBooksListRefreshing
        {
            get
            {
                return _IsComicBooksListRefreshing;
            }

            set
            {
                if (_IsComicBooksListRefreshing != value)
                {
                    _IsComicBooksListRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _Counter;
        public int Counter
        {
            get
            {
                return _Counter;
            }

            set
            {
                if (_Counter != value)
                {
                    _Counter = value;
                    OnPropertyChanged();
                }
            }
        }

        private ComicBook _SelectedComicBook;
        public ComicBook SelectedComicBook
        {
            get
            {
                return _SelectedComicBook;
            }

            set
            {
                if (_SelectedComicBook != value)
                {
                    _SelectedComicBook = value;
                    OnPropertyChanged();                    
                }
            }
        }

        private ObservableCollection<string> _Brands;
        public ObservableCollection<string> Brands
        {
            get
            {
                return _Brands;
            }

            set
            {
                if (_Brands != value)
                {
                    _Brands = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SelectedBrand;
        public string SelectedBrand
        {
            get
            {
                return _SelectedBrand;
            }

            set
            {
                if (_SelectedBrand != value)
                {
                    _SelectedBrand = value;
                    OnPropertyChanged();

                    FilterByBrand.Execute(string.IsNullOrEmpty(_SelectedBrand) || _SelectedBrand.Contains("-- All") ?
                       null : string.Format("brand:{0}", _SelectedBrand));
                }
            }
        }

        private ObservableCollection<string> _Series;
        public ObservableCollection<string> Series
        {
            get
            {
                return _Series;
            }

            set
            {
                if (_Series != value)
                {
                    _Series = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _SelectedSeries;
        public string SelectedSeries
        {
            get
            {
                return _SelectedSeries;
            }

            set
            {
                if (_SelectedSeries != value)
                {
                    _SelectedSeries = value;
                    OnPropertyChanged();

                    FilterBySeries.Execute(string.IsNullOrEmpty(_SelectedSeries) || _SelectedSeries.Contains("-- All") ? 
                        null : string.Format("series:{0}", _SelectedSeries));                    
                }
            }
        }

        public void UpdateView()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () => { 
                PrepareList(await AppShell.DataStore.GetLastAsync());
            });
        }

        public override Task OnViewAppearing(object parameter = null)
        {            
            return Task.Run(async () =>
            {
                IsLoading = true;
                
                if (AppShell.DataStore == null)
                {
                    AppShell.DataStore = await ComicBookDataStore.GetInstance();

                    if (await AppShell.DataStore.Any() == false)
                    {
                        var demo = await Shell.Current.DisplayAlert(
                            "Demo mode",
                            "No data found. Do you want to load example comic book data?",
                            "YES",
                            "Nope"
                        );

                        if (demo)
                        {
                            AppShell.DataStore = await MockDataStore.GetInstance();
                        }
                    }

                    PrepareList(await AppShell.DataStore.GetAsync(), true);
                }
                // 
                IsLoading = false;
            });
        }
        
        public ICommand DisplayFavorites => new Command(async () =>
            {
                if (currentContent != ListContent.FAVORITES) 
                {
                    currentContent = ListContent.FAVORITES;
                    PrepareList(await AppShell.DataStore.GetFavoritesAsync());
                }
                else
                {
                    currentContent = ListContent.ANY;                   
                    PrepareList(await AppShell.DataStore.GetAsync());
                }
            }
        );

        public ICommand DisplayOnShelf => new Command(async () =>
            {                
                if (currentContent == ListContent.ANY)
                {
                    currentContent = ListContent.ON_SHELF;                    
                    PrepareList(await AppShell.DataStore.GetOnShelfAsync());
                }
                else if (currentContent == ListContent.ON_SHELF)
                {
                    currentContent = ListContent.NOT_ON_SHELF;                    
                    PrepareList(await AppShell.DataStore.GetNotOnShelfAsync());
                }
                else
                {
                    currentContent = ListContent.ANY;                    
                    PrepareList(await AppShell.DataStore.GetAsync());
                }
            }
        );

        public ICommand FilterByBrand => new Command<string>(async (string s) => {
            currentContent = string.IsNullOrEmpty(s) ? ListContent.ANY : ListContent.BRANDS;
            PrepareList(await AppShell.DataStore.GetByQuery(s));
        });

        public ICommand FilterBySeries => new Command<string>(async (string s) => {
            currentContent = string.IsNullOrEmpty(s) ? ListContent.ANY : ListContent.SERIES;
            PrepareList(await AppShell.DataStore.GetByQuery(s));
        });

        public ICommand FilterByQuery => new Command<string>(async (string s) =>  {
            currentContent = string.IsNullOrEmpty(s) ? ListContent.ANY : ListContent.SEARCH_RESULTS;
            PrepareList(await AppShell.DataStore.GetByQuery(s));
        });

        public ICommand RefreshComicBooks => new Command(async () => {

            IsComicBooksListRefreshing = true;
            currentContent = ListContent.ANY;            
            PrepareList(await AppShell.DataStore.GetAsync());
            IsComicBooksListRefreshing = false;

        });
        
        public ICommand SelectGroupMode => new Command(async () =>
        {
            var groupBy = await App.Current.MainPage.DisplayActionSheet(
                "Grouping", "Cancel", null, new string[] { "Title","Brand", "Series", "Publisher" }
            );
            Xamarin.Essentials.Preferences.Set("CatalogGroupMode", groupBy);            
            PrepareList(await AppShell.DataStore.GetAsync());
        });

        public ICommand OpenView => new Command<string>(async (string uri) => {
            if (await AppShell.DataStore.Readonly() && (uri.Contains("details") == false && uri.Contains("about") == false))
            {
                await Shell.Current.DisplayAlert("Read only",
                    string.Format("Current datasource ({0}) is readonly.", await AppShell.DataStore.Name()),
                    "I understand"
                );
            }
            else
            {
                await Shell.Current.GoToAsync(uri);
            }
        });

        private void PrepareList(IEnumerable<ComicBook> comicBooks, bool init = false)
        {
            // 
            Title = currentContent switch
            {
                ListContent.ANY => "Catalog",
                ListContent.FAVORITES => "Favorites",
                ListContent.ON_SHELF => "On shelf",
                ListContent.NOT_ON_SHELF => "Not on shelf",
                ListContent.SEARCH_RESULTS => "Search results",
                ListContent.SERIES => "Series",
                ListContent.BRANDS => "Brands",
                _ => "Catalog"
            };

            // grupowanie na podstawie zapisanych przez użytkownika preferencji (domyslnie po indeksie)
            var group = Xamarin.Essentials.Preferences.Get("CatalogGroupMode", "Title");
            // sortowanie i grupowanie danych
            var grouped = group switch
            {
                "Title" => comicBooks.OrderBy(c => c.Title).ThenBy(c => c.Volume).GroupBy(c => c.Index),                
                "Brand" => comicBooks.OrderBy(c => c.Brand).ThenBy(c => c.Sequence).GroupBy(c => c.Brand),
                "Series" => comicBooks.OrderBy(c => c.Series).ThenBy(c => c.Sequence).GroupBy(c => c.Series),
                "Publisher" => comicBooks.OrderBy(c => c.Publisher).ThenBy(c => c.Sequence).GroupBy(c => c.Publisher),
                _ => comicBooks.OrderBy(c => c.Title).ThenBy(c => c.Volume).GroupBy(c => c.Index),
            };

            ComicBooks = new ObservableCollection<Grouping<string, ComicBook>>(
                grouped.Select(s => new Grouping<string, ComicBook>(s.Key, s))
            );

            //if (init)
            {
                var _series = new List<string>() { "-- All" };
                _series.AddRange(comicBooks.Where(c => string.IsNullOrEmpty(c.Series) == false)
                     .OrderBy(c => c.Series).Select(c => c.Series).Distinct().ToList());
                Series = new ObservableCollection<string>(_series);

                var _brands = new List<string>() { "-- All" };
                _brands.AddRange(comicBooks.Where(c => string.IsNullOrEmpty(c.Brand) == false)
                    .OrderBy(c => c.Brand).Select(c => c.Brand).Distinct().ToList());
                Brands = new ObservableCollection<string>(_brands);
            }

            Counter = comicBooks.Count();
        }

        public ICommand PrepareImport => new Command(async () =>
        {
            var res = await Xamarin.Essentials.FilePicker.PickAsync(null);
            if (res != null)
            {
                if (res.FileName.EndsWith("csv"))
                {
                    IsLoading = true;

                    string line;
                    List<ComicBook> import = new List<ComicBook>();
                    int counter = 0;
                    System.IO.StreamReader file = new System.IO.StreamReader(res.FullPath);
                    while ((line = file.ReadLine()) != null)
                    {
                        counter++;

                        // Tytuł;	Tom;	Marka;	Seria;	Na półce;	Autorzy;	Url;	Język;
                        var data = line.Split(';');
                        if (data.Length == 8)
                        {
                            try
                            {
                                import.Add(new ComicBook(true)
                                {
                                    UUID = Guid.NewGuid().ToString(),
                                    Title = data[0].Trim(),
                                    Volume = string.IsNullOrEmpty(data[1]) ? 0 : Convert.ToInt32(data[1]),
                                    Brand = data[2].Trim(),
                                    Series = data[3].Trim(),
                                    OnBookShelf = string.IsNullOrEmpty(data[4]) == false,
                                    Writers = data[5].Trim(),
                                    Url = data[6].Trim(),
                                    Language = data[7].Trim()
                                });
                            }
                            catch
                            {
                                // TODO
                            }
                        }
                    }

                    file.Close();
                    if (import.Count > 0)
                    {
                        var confirm = await Shell.Current.DisplayAlert("Yeah", string.Format("We found {0} record(s)", import.Count), "Import", "Cancel");
                        if (confirm)
                        {
                            if (await (await ComicBookDataStore.GetInstance()).AddAsync(import))
                            {
                                DependencyService.Get<IMessageService>().Success("Done");                                
                                IsLoading = false;
                            }
                        }
                        else
                        {
                            IsLoading = false;
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Yeah", "No records found", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Huh", "Only csv files are allowed", "OK");
                }
            }
        });

        public ICommand DisplaySeries => new Command(() => {
            MessagingCenter.Send(this, "series");
        });

        public ICommand DisplayBrands => new Command(() => {
            MessagingCenter.Send(this, "brands");
        });
    }
}
