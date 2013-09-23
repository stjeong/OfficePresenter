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
            string url = string.Format("http://{0}:{1}/getSnapshot", this.Application.IPSelected, this.Application.Port);

            App.CallUrl(url, SnapshotCompleted);
            EnableControls(false);
        }

        private void EnableControls(bool enable)
        {
            btnConnect.IsEnabled = enable;
            txtIP.IsEnabled = enable;
            txtPort.IsEnabled = enable;
        }

        void SnapshotCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            EnableControls(true);

            if (e.Error != null)
            {
                string url = string.Format("{0}:{1}", this.Application.IPSelected, this.Application.Port);

#if DEBUG2
                MessageBox.Show(e.Error.ToString());
#else
                MessageBox.Show("Can't connect to Desktop Application: " + url);
#endif
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

        private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            // MessageBox.Show(e.Error.ToString());
        }
    }
}