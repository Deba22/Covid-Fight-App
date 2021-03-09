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

namespace Covid19.ListviewClass
{
    public enum ListItemType
    {
        Header = 0,
        DataItem = 1,
        Status = 2
    }
}