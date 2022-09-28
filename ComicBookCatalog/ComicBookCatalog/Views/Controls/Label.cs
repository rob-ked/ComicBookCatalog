using System;
using System.Collections.Generic;
using System.Text;

namespace ComicBookCatalog.Views.Controls
{
    class LabelBlack : Xamarin.Forms.Label
    {
        public LabelBlack()
        {
            this.FontFamily = "_FBlack";
        }
    }

    class LabelBold : Xamarin.Forms.Label
    {
        public LabelBold()
        {
            this.FontFamily = "_FBold";
        }
    }

    class LabelRegular : Xamarin.Forms.Label
    {
        public LabelRegular()
        {
            this.FontFamily = "_FRegular";
        }
    }

    class LabelLight : Xamarin.Forms.Label
    {
        public LabelLight()
        {
            this.FontFamily = "_FLight";
        }
    }
}
