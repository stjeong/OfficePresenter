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
        Brush _originalBrush;

        public MainPage()
        {
            InitializeComponent();
#if DEBUG
            btnConnectClicked(null, null);
#endif
            _originalBrush = this.Foreground;


            Application.IPSelected = "192.168.0.25";
        }

        public App Application
        {
            get { return (App.Current as App); }
        }

        private void btnConnectClicked(object sender, RoutedEventArgs e)
        {
            string url = string.Format("http://{0}:{1}/getSnapshot", this.Application.IPSelected, this.Application.Port);

            TimeoutContext tc = new TimeoutContext();
            tc.Timeout = 5000;

            if (App.CallUrl(url, SnapshotCompleted, tc) == true)
            {
                EnableControls(false);
            }
        }

        private void EnableControls(bool enable)
        {
            if (enable == false)
            {
                this.Foreground = new SolidColorBrush(Colors.Gray);
                customIndeterminateProgressBar.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.Foreground = _originalBrush;
                customIndeterminateProgressBar.Visibility = System.Windows.Visibility.Collapsed;
            }

            btnConnect.IsEnabled = enable;
            txtIP.IsEnabled = enable;
            txtPort.IsEnabled = enable;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            EnableControls(true);
        }

        void SnapshotCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                EnableControls(true);
                return;
            }

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
                catch (Exception)
                {
                }
            }
        }

        private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            // MessageBox.Show(e.Error.ToString());
        }
    }
}