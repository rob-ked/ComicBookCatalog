using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ComicBookCatalog.Base;
using ComicBookCatalog.Extensions;
using ComicBookCatalog.Models;
using SQLite;
using Xamarin.Essentials;

namespace ComicBookCatalog.Services
{
    public class MockDataStore : IDataStore<ComicBook>
    {
        /// <summary>
        ///  
        /// </summary>
        private static MockDataStore dataStoreInstance;

        /// <summary>
        /// 
        /// </summary>
        private static List<ComicBook> comicBooks;

        /// <summary>
        /// 
        /// </summary>
        private MockDataStore()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<MockDataStore> GetInstance()
        {
            if (dataStoreInstance != null)
            {
                return dataStoreInstance;
            }

            dataStoreInstance = new MockDataStore();
            await dataStoreInstance.LoadAsync();

            return dataStoreInstance;
        }
        
        private async Task LoadAsync()
        {            
            using (var stream = typeof(MockDataStore).Assembly.GetManifestResourceStream("ComicBookCatalog.Samples.ComicBooks.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    comicBooks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ComicBook>>(await reader.ReadToEndAsync());
                }
            }
        }

        #region IDataStore
                
        public async Task<bool> AddAsync(ComicBook comicBook)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddAsync(List<ComicBook> comicBooks)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(ComicBook comicBook)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveAsync(string id)
        {
            throw new NotImplementedException();
        }
        
        public async Task<IEnumerable<ComicBook>> GetAsync()
        {
            return await Task.FromResult(comicBooks == null ?
                new List<ComicBook>() :
                comicBooks
            );
        }

        public async Task<IEnumerable<ComicBook>> GetLastAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ComicBook>> GetByQuery(string q)
        {
            // nie ma danych - zwracamy pusta listę
            if (comicBooks == null)
            {
                return new List<ComicBook>();
            }

            // nie ma zapytania - zwracamy całą listę
            if (string.IsNullOrEmpty(q))
            {
                return await GetAsync();
            }

            // 
            string property = null;

            //
            string query = null;

            //
            List<ComicBook> result = null;

            if (q.Contains(":"))
            {
                property = q.Substring(0, q.IndexOf(":"));
                query = q.Substring(q.IndexOf(":") + 1);
            }
            else
            {
                query = q;
            }

            Func<ComicBook, bool> condition = property switch
            {
                "title" => c => c.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0,               
                "brand" => c => string.IsNullOrEmpty(c.Brand) == false && c.Brand.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0,
                "series" => c => string.IsNullOrEmpty(c.Series) == false && c.Series.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0,
                "writer" => c => string.IsNullOrEmpty(c.Writers) == false && c.Writers.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0,
                _ => c => c.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||                    
                    (string.IsNullOrEmpty(c.Brand) == false && c.Brand.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (string.IsNullOrEmpty(c.Writers) == false && c.Writers.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (string.IsNullOrEmpty(c.Series) == false && c.Series.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0),
            };

            result = comicBooks.Where(condition).ToList();

            if (result.Any() == false)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert
                    (
                        "Sorry",
                        "Search result is empty. Please try to change your search query.",
                        "Oh my..."
                    );
                });
            }
            
            return await Task.FromResult(result);
        }
        
        public async Task<ComicBook> GetAsync(string uuid)
        {
            return await Task.FromResult(comicBooks.FirstOrDefault(s => s.UUID == uuid));
        }

        public async Task<bool> Any()
        {
            return await Task.FromResult(comicBooks.Any());
        }

        public async Task<IEnumerable<ComicBook>> GetFavoritesAsync()
        {
            return await Task.FromResult(comicBooks == null ?
                new List<ComicBook>() :
                comicBooks.Where(c => c.Favorite)
            );
        }

        public async Task<IEnumerable<ComicBook>> GetOnShelfAsync()
        {
            return await Task.FromResult(comicBooks == null ?
                 new List<ComicBook>() :
                 comicBooks.Where(c => c.OnBookShelf)
             );
        }

        public async Task<IEnumerable<ComicBook>> GetNotOnShelfAsync()
        {
            return await Task.FromResult(comicBooks == null ?
                new List<ComicBook>() :
                comicBooks.Where(c => c.OnBookShelf == false)
            );
        }

        public Task<bool> Readonly()
        {
            return Task.FromResult(true);
        }

        public Task<string> Name()
        {
            return Task.FromResult("Demo");
        }

        #endregion
    }
}