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

            string ipFrom = Intent.GetStringExtra("ip");
            string portFrom = Intent.GetStringExtra("port");

            if (string.IsNullOrEmpty(ipFrom) == false)
            {
                txtIP.Text = ipFrom;
            }

            if (string.IsNullOrEmpty(portFrom) == false)
            {
                txtPort.Text = portFrom;
            }

#if DEBUG
            txtIP.Text = "192.168.1.84";
#else
        //    txtIP.Text = "192.168.1.84";
#endif
        }

        void btnConnect_Click(object sender, EventArgs e)
        {
            string url = string.Format("http://{0}:{1}/getSnapshot", txtIP.Text, txtPort.Text);

            TimeoutContext tc = new TimeoutContext();
            tc.Timeout = 5000;

            if (App.CallUrl(url, SnapshotCompleted, tc) == true)
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
            string url = string.Format("{0}:{1}", txtIP.Text, txtPort.Text);

            if (e.Cancelled == true)
            {
                ShowToastMessage("Can't connect to Desktop Application: " + url);
                return;
            }

            if (e.Error != null)
            {

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

                        //Bundle bundle = new Bundle();
                        //SerializableString result = new SerializableString();
                        //result.Text = e.Result;
                        //bundle.PutSerializable("document", result);
                        // i.PutExtra("document", e.Result);

                        App.DocumentText = e.Result;
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

