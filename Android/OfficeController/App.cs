using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OfficeController
{
    public class App
    {
        static string _documentText;
        public static string DocumentText
        {
            get { return _documentText; }
            set { _documentText = value; }
        }

        public static bool CallUrl(string url, DownloadStringCompletedEventHandler handler)
        {
            WebClient wc = new WebClient();
            // wc.Headers[HttpRequestHeader.IfModifiedSince] = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            if (handler != null)
            {
                wc.DownloadStringCompleted += handler;
            }

            Uri uri = null;

            try
            {
                uri = new Uri(url);
            }
            catch (FormatException)
            {
                return false;
            }

            wc.DownloadStringAsync(uri, null);

            return true;
        }
    }
}