using ComicBookCatalog.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComicBookCatalog.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataStore<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        Task<bool> AddAsync(T i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        Task<bool> AddAsync(List<T> i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T i);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string uuid);
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetLastAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetFavoritesAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetOnShelfAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetNotOnShelfAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        Task<T> GetAsync(string uuid);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetByQuery(string querys);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> Any();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<bool> Readonly();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> Name();
    }
}
