using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ComicBookCatalog.Droid.Platform;

[assembly: Xamarin.Forms.Dependency(typeof(MessageServiceAndroid))]
namespace ComicBookCatalog.Droid.Platform
{
    class MessageServiceAndroid : ComicBookCatalog.Services.IMessageService
    {
        public void Error(string text)
        {
            Toast.MakeText(Application.Context, text, ToastLength.Long).Show();
        }

        public void Success(string text)
        {
            Toast.MakeText(Application.Context, text, ToastLength.Short).Show();
        }
    }
}