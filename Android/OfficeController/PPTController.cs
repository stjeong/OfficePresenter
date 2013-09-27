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

namespace OfficeController
{
    [Activity(Label = "My Activity")]
    public class PPTController : Activity
    {
        TextView txtTitle;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.PPTController);

            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

            string ip = Intent.GetStringExtra("ip");
            string port = Intent.GetStringExtra("port");
            string document = Intent.GetStringExtra("document");
        }
    }
}