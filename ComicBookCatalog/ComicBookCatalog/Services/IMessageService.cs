using System;
using System.Collections.Generic;
using System.Text;

namespace ComicBookCatalog.Services
{
    public interface IMessageService
    {
        void Success(string text);

        void Error(string text);
    }
}
