using ComicBookCatalog.Models;
using ComicBookCatalog.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ComicBookCatalog.ViewModels
{
    class Details : Base.ViewModel
    {
        public Details()
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
                    OnPropertyChanged(nameof(FavoriteButtonColor));
                    OnPropertyChanged(nameof(OnBookShelfButtonColor));
                }
            }
        }

        public void Refresh(ComicBook comicBook)
        {
            ComicBook = comicBook;
        }

        public string FavoriteButtonColor
        {
            get
            {
                return ComicBook != null && ComicBook.Favorite ? "#e84118" : "#2c3e50";
            }
        }

        public string OnBookShelfButtonColor
        {
            get
            {
                return ComicBook != null && ComicBook.OnBookShelf ? "#3e517a" : "#2c3e50";
            }
        }

        public override Task OnViewAppearing(object parameter = null)
        {
            if (parameter != null)
            {
                return Task.Run(async () =>
                {
                    ComicBook = await AppShell.DataStore.GetAsync(parameter.ToString());
                    if (ComicBook == null)
                    {
                        DependencyService.Get<IMessageService>().Error("Comic book not found");
                        MessagingCenter.Send(this, "close");
                    }
                });
            }
            else
            {
                throw new Exception("Comic book uuid is missing");
            }
        }

        public ICommand GoBack => new Command(async () =>
        {
            await Shell.Current.GoToAsync("..");
        });

        public ICommand DisplayCoverImage => new Command(async () =>
        {
            if (ComicBook.CanDisplayCoverImage)
            {
                await Shell.Current.GoToAsync(string.Format("cover?uuid={0}", ComicBook.UUID));
            }
        });

        public ICommand ToggleFavorite => new Command(async () =>
        {
            if (await AppShell.DataStore.Readonly())
            {
                await Shell.Current.DisplayAlert("Read only",
                    string.Format("Current datasource ({0}) is readonly.", await AppShell.DataStore.Name()),
                    "I understand"
                );

                return;
            }

            ComicBook.Favorite = !ComicBook.Favorite;
            OnPropertyChanged(nameof(FavoriteButtonColor));
            if (await AppShell.DataStore.UpdateAsync(ComicBook))
            {
                DependencyService.Get<IMessageService>().Success("Done");
                MessagingCenter.Send(this, "update");
            }
            else
            {
                DependencyService.Get<IMessageService>().Error("Something went wrong");
            }
        });

        public ICommand ToggleOnBookShelf => new Command(async () =>
        {
            if (await AppShell.DataStore.Readonly())
            {
                await Shell.Current.DisplayAlert("Read only",
                    string.Format("Current datasource ({0}) is readonly.", await AppShell.DataStore.Name()),
                    "I understand"
                );

                return;
            }

            ComicBook.OnBookShelf = !ComicBook.OnBookShelf;
            OnPropertyChanged(nameof(OnBookShelfButtonColor));
            if (await AppShell.DataStore.UpdateAsync(ComicBook))
            {
                DependencyService.Get<IMessageService>().Success("Done");
                MessagingCenter.Send(this, "update");
            }
            else
            {
                DependencyService.Get<IMessageService>().Error("Something went wrong");
            }            
        });

        public ICommand Edit => new Command(async () =>
        {
            if (await AppShell.DataStore.Readonly())
            {
                await Shell.Current.DisplayAlert("Read only",
                    string.Format("Current datasource ({0}) is readonly.", await AppShell.DataStore.Name()),
                    "I understand"
                );

                return;
            }
            
            await Shell.Current.GoToAsync(string.Format("edit?uuid={0}", ComicBook.UUID));            
        });

        public ICommand Delete => new Command(async () =>
        {
            if (await AppShell.DataStore.Readonly())
            {
                await Shell.Current.DisplayAlert("Read only",
                    string.Format("Current datasource ({0}) is readonly.", await AppShell.DataStore.Name()),
                    "I understand"
                );

                return;
            }
                
            if (await Shell.Current.DisplayAlert("Confirm", "Do you want to remove this comic book?", "Yes", "No"))
            {
                if (await AppShell.DataStore.RemoveAsync(ComicBook.UUID))
                {
                    ComicBook = null;
                    MessagingCenter.Send(this, "close");
                }
            }            
        });

        public ICommand GoToUrl => new Command(async () =>
        {
            if (string.IsNullOrEmpty(ComicBook.Url) == false)
            {
                await Xamarin.Essentials.Browser.OpenAsync(new Uri(ComicBook.Url), Xamarin.Essentials.BrowserLaunchMode.External);
            }            
        });
    }
}
