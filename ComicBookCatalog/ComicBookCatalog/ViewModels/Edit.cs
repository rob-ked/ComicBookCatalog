using ComicBookCatalog.Base;
using ComicBookCatalog.Models;
using ComicBookCatalog.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace ComicBookCatalog.ViewModels
{
    class Edit : Base.ViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        private MediaFile CoverImageFile;

        public Edit()
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

                    CoverImage = _ComicBook.CoverImage;
                }
            }
        }

        private ImageSource _CoverImage;
        public ImageSource CoverImage
        {
            get
            {
                return _CoverImage;
            }

            set
            {
                if (_CoverImage != value)
                {
                    _CoverImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _AreExtraFieldsVisible;
        public bool AreExtraFieldsVisible
        {
            get
            {
                return _AreExtraFieldsVisible;
            }

            set
            {
                if (_AreExtraFieldsVisible != value)
                {
                    _AreExtraFieldsVisible = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsToggleExtraFieldVisibilityButtonVisible));
                }
            }
        }

        public bool IsToggleExtraFieldVisibilityButtonVisible
        {
            get
            {
                return _AreExtraFieldsVisible == false;
            }
        }

        public override Task OnViewAppearing(object parameter = null)
        {
            if (parameter == null)
            {
                return Task.Run(() => {
                    ComicBook = new ComicBook(true);                    
                    Title = "Create"; 
                });
            }
            else 
            { 
                return Task.Run(async () =>
                {
                    var c = await AppShell.DataStore.GetAsync(parameter.ToString());
                    if (c == null)
                    {
                        ComicBook = new ComicBook(true);                        
                        Title = "Create";
                    }
                    else
                    {
                        ComicBook = c;                         
                        Title = "Update";
                    }
                });
            }
        }

        public ICommand Save => new Command(async () =>
        {
            bool allowToSaveOrUpdate = true;
            List<string> validateErrorsMessage = new List<string>();

            if (string.IsNullOrEmpty(ComicBook.Title))
            {
                allowToSaveOrUpdate = false;
                validateErrorsMessage.Add("- Title is required");
            }

            if (string.IsNullOrEmpty(ComicBook.Writers))
            {
                allowToSaveOrUpdate = false;
                validateErrorsMessage.Add("- Writer is required");
            }

            if (AreExtraFieldsVisible)
            {
                if (string.IsNullOrEmpty(ComicBook.Publisher))
                {
                    allowToSaveOrUpdate = false;
                    validateErrorsMessage.Add("- Publisher is required");
                }

                if (string.IsNullOrEmpty(ComicBook.Language))
                {
                    allowToSaveOrUpdate = false;
                    validateErrorsMessage.Add("- Language is required");
                }
                else
                {
                    if (ComicBook.Language.Length > 2)
                    {
                        allowToSaveOrUpdate = false;
                        validateErrorsMessage.Add("- Language is invalid. Two letters code is required");
                    }
                }
            }

            if (string.IsNullOrEmpty(ComicBook.Url) == false)
            {
                if (Uri.TryCreate(ComicBook.Url, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttp)
                {
                    allowToSaveOrUpdate = false;
                    validateErrorsMessage.Add("- Url is invalid");
                }
            }

            if (allowToSaveOrUpdate)
            {
                if (CoverImageFile != null)
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var filename = string.Format("cover_{0}_{1}.jpg", ComicBook.UUID, Guid.NewGuid());
                    var filepath = Path.Combine(path, filename);

                    File.Copy(CoverImageFile.Path, filepath, true);
                    ComicBook.Cover = filename;
                }

                ComicBook.Updated = DateTime.Now;

                if (ComicBook.Temporary == true)
                {
                    if (await AppShell.DataStore.AddAsync(ComicBook))
                    {
                        DependencyService.Get<IMessageService>().Success("Saved");
                        MessagingCenter.Send(this, "update");
                        ComicBook = new ComicBook(true);
                        CoverImageFile = null;
                    }
                }
                else
                {
                    if (await AppShell.DataStore.UpdateAsync(ComicBook))
                    {
                        DependencyService.Get<IMessageService>().Success("Saved");
                        MessagingCenter.Send(this, "update");
                        await Shell.Current.Navigation.PopAsync();
                    }
                }
            }
            else
            {
                await AppShell.Current.DisplayAlert("Hmmm... ", 
                    string.Join(Environment.NewLine, validateErrorsMessage), "OK");
            }

        });

        public ICommand SelectCoverImage => new Command(async () =>
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                CoverImageFile = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    MaxWidthHeight = 600,
                    CompressionQuality = 80,
                    ModalPresentationStyle = MediaPickerModalPresentationStyle.FullScreen
                });

                if (CoverImageFile != null)
                {
                    CoverImage = ImageSource.FromStream(() => { return CoverImageFile.GetStream(); });
                }
            }
        });

        public ICommand TakeCoverPicture => new Command(async () =>
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsTakePhotoSupported && CrossMedia.Current.IsCameraAvailable)
            {
                CoverImageFile = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                {
                    MaxWidthHeight = 600,
                    CompressionQuality = 80,
                    ModalPresentationStyle = MediaPickerModalPresentationStyle.FullScreen,
                    AllowCropping = true                   
                });

                if (CoverImageFile != null)
                {
                    CoverImage = ImageSource.FromStream(() => { return CoverImageFile.GetStream(); });
                }
            } 
            else
            {
                await App.Current.MainPage.DisplayAlert("Nooo...", "No camera available", "OK");
            }
        });

        public ICommand ToggleExtraFieldVisibility => new Command(() =>
        {
            AreExtraFieldsVisible = true;
        });

        public ICommand GoBack => new Command(async () =>
        {
            await Shell.Current.Navigation.PopAsync();
        });
    }
}
