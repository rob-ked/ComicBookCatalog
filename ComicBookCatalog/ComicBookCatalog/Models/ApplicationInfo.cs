using System;
using System.Collections.Generic;
using System.Text;

namespace ComicBookCatalog.Models
{
    class ApplicationInfo
    {
        public string Version 
        {
            get
            {
                return string.Format("Version {0} - {1}", Base.Config.VersionNumber, Base.Config.VersionCodeName);
            }
        }

        public string PlatformName
        {
            get
            {
                return string.Format("Platform: {0}", Xamarin.Essentials.DeviceInfo.Name);
            }
        }

        public string PlatformVersion
        {
            get
            {
                return string.Format("Version: {0}", Xamarin.Essentials.DeviceInfo.VersionString);
            }
        }

        public string PlatformModel
        {
            get
            {
                return string.Format("Device: {0} manufactured by {1}", Xamarin.Essentials.DeviceInfo.Model, Xamarin.Essentials.DeviceInfo.Manufacturer);
            }
        }
    }
}
