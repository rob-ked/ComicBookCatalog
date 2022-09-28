﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ComicBookCatalog.Base
{
    class Config
    {
        /// <summary>
        /// Numer wersji aplikacji
        /// </summary>
        public const string VersionNumber = "0.5";

        /// <summary>
        /// Nazwa kodowa wersji aplikacji
        /// </summary>
        public const string VersionCodeName = "Nailbiter";

        /// <summary>
        /// 
        /// </summary>
        public const string CoverImagePlaceholder = "/9j/4QAYRXhpZgAASUkqAAgAAAAAAAAAAAAAAP/sABFEdWNreQABAAQAAABQAAD/4QMraHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCI/PiA8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4OnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA1LjAtYzA2MSA2NC4xNDA5NDksIDIwMTAvMTIvMDctMTA6NTc6MDEgICAgICAgICI+IDxyZGY6UkRGIHhtbG5zOnJkZj0iaHR0cDovL3d3dy53My5vcmcvMTk5OS8wMi8yMi1yZGYtc3ludGF4LW5zIyI+IDxyZGY6RGVzY3JpcHRpb24gcmRmOmFib3V0PSIiIHhtbG5zOnhtcD0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLyIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBob3Rvc2hvcCBDUzUuMSBXaW5kb3dzIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjcxQjVDNkFGQzQ0MTExRTRCOEIyRkU3OUM3OTRGMTI3IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjcxQjVDNkIwQzQ0MTExRTRCOEIyRkU3OUM3OTRGMTI3Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6NzFCNUM2QURDNDQxMTFFNEI4QjJGRTc5Qzc5NEYxMjciIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6NzFCNUM2QUVDNDQxMTFFNEI4QjJGRTc5Qzc5NEYxMjciLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz7/7gAOQWRvYmUAZMAAAAAB/9sAhAACAgICAgICAgICAwICAgMEAwIC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";

        /// <summary>
        /// 
        /// </summary>
        public static class DB
        {
            /// <summary>
            /// Nazwa pliku bazy danych
            /// </summary>
            public static string DBFileName = "ComicBookCatalog.DB.v0.1.db3";

            public const SQLite.SQLiteOpenFlags DBFlags =
            SQLite.SQLiteOpenFlags.ReadWrite |
            SQLite.SQLiteOpenFlags.Create |            
            SQLite.SQLiteOpenFlags.SharedCache;

            public static string DBPath
            {
                get
                {
                    return Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                        DBFileName
                    );
                }
            }
        }
    }
}