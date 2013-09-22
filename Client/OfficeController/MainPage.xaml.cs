using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.Text;
using OfficeInterface;
using System.Runtime.Serialization.Json;
using Microsoft.Phone.Shell;

namespace OfficeController
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;

#if DEBUG
            btnConnectClicked(null, null);
#endif
        }

        public App Application
        {
            get { return (App.Current as App); }
        }

        private void btnConnectClicked(object sender, RoutedEventArgs e)
        {
            string url = string.Format("http://{0}:{1}/getSnapshot&junk={2}", this.Application.IPSelected, this.Application.Port,
                DateTime.Now);
            Uri uri = new Uri(url);

            WebClient wc = new WebClient();

            wc.Headers[HttpRequestHeader.CacheControl] = "no-cache";
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(SnapshotCompleted);
            wc.DownloadStringAsync(uri);
        }

        void SnapshotCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(e.Result) == false)
                    {
                        PhoneApplicationService.Current.State["Document"] = e.Result;
                        string url = string.Format("/PPTController.xaml?ip={0}&port={1}",
                            this.Application.IPSelected, this.Application.Port);
                        this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}