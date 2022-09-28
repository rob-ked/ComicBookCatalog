using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;

namespace ComicBookCatalog.Models
{
    public class Grouping<K, T> : ObservableCollection<T>
    {
        public K Index { get; private set; }

        public Grouping(K index, IEnumerable<T> items)
        {
            Index = index;
            foreach(var i in items)
            {
                this.Items.Add(i);
            }
        }
    }
}