using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ComicBookCatalog.Base;
using ComicBookCatalog.Extensions;
using ComicBookCatalog.Models;
using Polly;
using SQLite;
using Xamarin.Essentials;

namespace ComicBookCatalog.Services
{
    public class ComicBookDataStore : IDataStore<ComicBook>
    {
        /// <summary>
        /// 
        /// </summary>
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Config.DB.DBPath, Config.DB.DBFlags);
        });

        /// <summary>
        /// 
        /// </summary>
        static SQLiteAsyncConnection Database => lazyInitializer.Value;

        /// <summary>
        /// 
        /// </summary>
        static bool initialized = false;

        /// <summary>
        ///  
        /// </summary>
        private static ComicBookDataStore dataStoreInstance;

        /// <summary>
        /// 
        /// </summary>
        private ComicBookDataStore()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        /// <summary>
        /// Metoda zwracająca instancję klasy
        /// </summary>
        /// <returns></returns>
        public static async Task<ComicBookDataStore> GetInstance()
        {
            if (dataStoreInstance == null) {
                dataStoreInstance = new ComicBookDataStore();
            }
            
            return dataStoreInstance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(ComicBook).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(ComicBook)).ConfigureAwait(false);
                }
                initialized = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="numRetries"></param>
        /// <returns></returns>
        protected static Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 10)
        {
            return Policy.Handle<SQLiteException>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action);
            static TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
        }

        #region IDataStore

        public async Task<bool> AddAsync(ComicBook comicBook)
        {
            if (await Database.InsertAsync(comicBook) > 0)
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> AddAsync(List<ComicBook> comicBooks)
        {
            if (await Database.InsertAllAsync(comicBooks, true) > 0)
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> UpdateAsync(ComicBook comicBook)
        {
            if (await Database.UpdateAsync(comicBook) > 0)
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public async Task<bool> RemoveAsync(string id)
        {
            if (await Database.DeleteAsync<ComicBook>(id) > 0)
            {
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

        public async Task<IEnumerable<ComicBook>> GetAsync()
        {
            var result = await Database.Table<ComicBook>().ToListAsync();
            if (result.Any() == false)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert
                    (
                        "Oh my...",
                        "Database is empty. Please try to create new item or import some data.",
                        "OK"
                    );
                });
            }

            return result;
        }

        public async Task<IEnumerable<ComicBook>> GetLastAsync()
        {
            var lastHour = DateTime.Now.AddHours(-1);
            return await Database.Table<ComicBook>().Where(c => c.Updated > lastHour).ToListAsync();            
        }

        public async Task<IEnumerable<ComicBook>> GetByQuery(string q)
        {
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
            IEnumerable<ComicBook> result = null;

            if (q.Contains(":"))
            {
                property = q.Substring(0, q.IndexOf(":"));
                query = q.Substring(q.IndexOf(":") + 1).ToLower();
            }
            else
            {
                query = q.ToLower();
            }

            Expression<Func<ComicBook, bool>> condition = property switch
            {
                "title" => c => c.Title.ToLower().Contains(query),
                "brand" => c => c.Brand != null && c.Brand.ToLower().Contains(query),
                "series" => c => c.Series != null && c.Series.ToLower().Contains(query),
                "writer" => c => c.Writers != null && c.Writers.ToLower().Contains(query),
                _ => c => c.Title.ToLower().Contains(query) || c.Brand.ToLower().Contains(query) || c.Series.ToLower().Contains(query) || c.Writers.ToLower().Contains(query)
            };

            result = await Database.Table<ComicBook>().Where(condition).ToListAsync();

            if (result.Any() == false)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert
                    (
                        "Oh my...",
                        "We can not find anything. Please try to change your search query.",
                        "OK"
                    );
                });
            }

            return await Task.FromResult(result);
        }

        public async Task<ComicBook> GetAsync(string uuid)
        {
            try
            {
                return await Database.Table<ComicBook>().FirstAsync(c => c.UUID == uuid);
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Any()
        {
            return await Database.Table<ComicBook>().CountAsync() > 0;
        }

        public async Task<IEnumerable<ComicBook>> GetFavoritesAsync()
        {
            return await Database.Table<ComicBook>().Where(c => c.Favorite).ToListAsync();
        }

        public async Task<IEnumerable<ComicBook>> GetOnShelfAsync()
        {
            return await Database.Table<ComicBook>().Where(c => c.OnBookShelf).ToListAsync();
        }

        public async Task<IEnumerable<ComicBook>> GetNotOnShelfAsync()
        {
            return await Database.Table<ComicBook>().Where(c => c.OnBookShelf == false).ToListAsync();
        }

        public Task<bool> Readonly()
        {
            return Task.FromResult(false);
        }

        public Task<string> Name()
        {
            return Task.FromResult("ComicBooks");
        }

        #endregion
    }
}