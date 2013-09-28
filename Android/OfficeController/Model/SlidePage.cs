using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OfficeController.Model
{
    public class SlidePage
    {
        public int Id { get; set; }
        public Bitmap Image { get; set; }
        public string Memo { get; set; }
    }
}