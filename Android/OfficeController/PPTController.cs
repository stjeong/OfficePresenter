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
using Cheesebaron.HorizontalListView;
using OfficeController.Model;
using OfficeInterface;

namespace OfficeController
{
    // https://github.com/xamarin/mobile-samples/blob/master/StandardControls/AndroidStandardControls/Screens/ContentControls/GalleryScreen.cs
    // https://github.com/Cheesebaron/Cheesebaron.HorizontalListView
    [Activity(Label = "OfficeController")]
    public class PPTController : Activity
    {
        HorizontalListView _bottomSlideList;
        HorizontalListView _panoramaGallery;
        PPTDocument _pptDocument;

        string _ip;
        string _port;

        List<SlidePage> _slideList;
        public List<SlidePage> SlideList
        {
            get { return _slideList; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.PPTController);

            _bottomSlideList = FindViewById<HorizontalListView>(Resource.Id.bottomSlideList);
            _panoramaGallery = FindViewById<HorizontalListView>(Resource.Id.panoramaSlideList);

            _ip = Intent.GetStringExtra("ip");
            _port = Intent.GetStringExtra("port");
            string document = Intent.GetStringExtra("document");

            _pptDocument = Newtonsoft.Json.JsonConvert.DeserializeObject<PPTDocument>(document);

            if (_pptDocument != null)
            {
                LoadDocument();
            }

            _bottomSlideList.Adapter = new BottomItemAdapter(this, Resource.Layout.bottomRow);
            _panoramaGallery.Adapter = new PanoramaItemAdapter(this, Resource.Layout.panoramaRow);

            _panoramaGallery.ItemSelected += _panoramaGallery_ItemSelected;
        }

        void _panoramaGallery_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
        }

        private void LoadDocument()
        {
            _slideList = new List<SlidePage>();
            int index = 0;

            foreach (var item in this._pptDocument.List)
            {
                byte [] imgContents = Convert.FromBase64String(item.ImageAsText);
                Bitmap bitmap = BitmapFactory.DecodeByteArray(imgContents, 0, imgContents.Length);

                SlidePage page = new SlidePage();
                page.Image = bitmap;
                page.Memo = item.Note;
                page.Id = index;

                index++;

                _slideList.Add(page);
            }
        }

        internal void SetSlide(int position)
        {
            _bottomSlideList.SetSelection(position);

            string url = string.Format("http://{0}:{1}/setSlide/{2}", _ip, _port, position + 1);
            App.CallUrl(url, null);
        }
    }
}