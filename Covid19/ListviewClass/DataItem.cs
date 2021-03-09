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
    public class DataItem : IListItem
    {
        public DataItem(string title, string subtitle,int rssi)
        {
            Text = title;
            SubTitle = subtitle;
            Rssi = rssi;
        }

        public string SubTitle { get; }

        public string Text { get; set; }

        public float Rssi { get; set; }

        public ListItemType GetListItemType()
        {
            return ListItemType.DataItem;
        }
    }
}