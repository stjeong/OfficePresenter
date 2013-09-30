using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

        public static bool CallUrl(string url, DownloadStringCompletedEventHandler handler, TimeoutContext tc)
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

            wc.DownloadStringAsync(uri, tc);
            if (tc != null)
            {
                wc.DownloadProgressChanged += tc.wc_DownloadProgressChanged;
                tc.Tag = wc;

                ThreadPool.QueueUserWorkItem(
                    (obj) =>
                    {
                        TimeoutContext timeContext = obj as TimeoutContext;
                        if (timeContext == null)
                        {
                            return;
                        }

                        WebClient wcInProgress = timeContext.Tag as WebClient;
                        if (wcInProgress == null)
                        {
                            return;
                        }

                        Thread.Sleep(timeContext.Timeout);
                        if (timeContext.Connected == true)
                        {
                            return;
                        }

                        if (timeContext.Completed == false)
                        {
                            wcInProgress.CancelAsync();
                        }
                    }, tc);
            }

            return true;
        }
    }
}