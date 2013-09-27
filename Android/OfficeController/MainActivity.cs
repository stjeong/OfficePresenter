using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;

namespace OfficeController
{
    [Activity(Label = "OfficeController", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Button btnConnect;
        TextView txtIP;
        TextView txtPort;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            btnConnect = FindViewById<Button>(Resource.Id.btnConnect);
            txtIP = FindViewById<TextView>(Resource.Id.txtIP);
            txtPort = FindViewById<TextView>(Resource.Id.txtPort);

            btnConnect.Click += btnConnect_Click;

#if DEBUG
            txtIP.Text = "192.168.0.19";
#endif
        }

        void btnConnect_Click(object sender, EventArgs e)
        {

            string url = string.Format("http://{0}:{1}/getSnapshot", txtIP.Text, txtPort.Text);
            if (App.CallUrl(url, SnapshotCompleted) == true)
            {
                EnableControls(false);
            }
        }

        private void EnableControls(bool enable)
        {
            RunOnUiThread(() =>
            {
                btnConnect.Enabled = enable;
                txtIP.Enabled = enable;
                txtPort.Enabled = enable;
            });
        }

        void SnapshotCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            EnableControls(true);

            if (e.Cancelled == true)
            {
                return;
            }

            if (e.Error != null)
            {
                string url = string.Format("{0}:{1}", txtIP.Text, txtPort.Text);

#if DEBUG
                ShowToastMessage(e.Error.ToString());
#else
                ShowToastMessage("Can't connect to Desktop Application: " + url);
#endif
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(e.Result) == false)
                    {
                        Intent i = new Intent(this, typeof(PPTController));

                        i.PutExtra("ip", txtIP.Text);
                        i.PutExtra("port", txtPort.Text);
                        i.PutExtra("document", e.Result);

                        StartActivity(i);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void ShowToastMessage(string msg)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, msg, ToastLength.Short).Show();
            });
        }

    }
}

