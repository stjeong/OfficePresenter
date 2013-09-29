using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
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
    public class PPTController : Activity, View.IOnTouchListener
    {
        BorderedImageView _imageView;
        TextView _txtMemo;

        BottomHorizontalListView _bottomSlideList;
        PPTDocument _pptDocument;

        int _currentSlideIndex;
        public int CurrentSlideIndex
        {
            get { return _currentSlideIndex; }
        }

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

            _imageView = FindViewById<BorderedImageView>(Resource.Id.panoramaImage);
            _txtMemo = FindViewById<TextView>(Resource.Id.txtMemo);
            _bottomSlideList = FindViewById<BottomHorizontalListView>(Resource.Id.bottomSlideList);

            _ip = Intent.GetStringExtra("ip");
            _port = Intent.GetStringExtra("port");

            //string document = Intent.GetStringExtra("document");
            string document = App.DocumentText;

            _pptDocument = Newtonsoft.Json.JsonConvert.DeserializeObject<PPTDocument>(document);

            if (_pptDocument != null)
            {
                LoadDocument();
            }

            StartShow(this.SlideList[0].AnimationCount);
            SetSlide(0);

            _bottomSlideList.Adapter = new BottomItemAdapter(this, Resource.Layout.bottomRow);
            _imageView.SetOnTouchListener(this);
        }

        private void LoadDocument()
        {
            _slideList = new List<SlidePage>();
            int index = 0;

            foreach (var item in this._pptDocument.List)
            {
                byte [] imgContents = Convert.FromBase64String(item.ImageAsText);

                BitmapFactory.Options opt = new BitmapFactory.Options();
                opt.InSampleSize = 1;
                opt.InPurgeable = true;
                opt.InDither = true;

                Bitmap bitmap = BitmapFactory.DecodeByteArray(imgContents, 0, imgContents.Length, opt);

                SlidePage page = new SlidePage();
                page.Image = bitmap;
                page.Memo = item.Note;
                page.Id = index;
                page.AnimationCount = item.AnimationCount;
                page.AnimationRemains = page.AnimationCount;

                index++;

                _slideList.Add(page);
            }
        }

        private void StartShow(int animationCount)
        {
            _countOfAnimation = animationCount;
            _currentAnimation = 0;

            string url = string.Format("http://{0}:{1}/startShow", _ip, _port);
            App.CallUrl(url, null);
        }

        internal void SetSlide(int position)
        {
            _countOfAnimation = _slideList[position].AnimationCount;
            _currentAnimation = 0;
            _currentSlideIndex = position;

            _imageView.SetImageBitmap(_slideList[position].Image);
            _txtMemo.Text = _slideList[position].Memo;

            string url = string.Format("http://{0}:{1}/setSlide/{2}", _ip, _port, position + 1);
            App.CallUrl(url, null);
        }

        float _downX;
        int _clickCount;
        DateTime _startTime;

        const int MaxDuration = 500;

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                _downX = e.GetX();

                if (_clickCount == 0)
                {
                    _startTime = DateTime.Now;
                    _clickCount++;
                }
                else if (_clickCount == 1)
                {
                    TimeSpan ts = DateTime.Now - _startTime;
                    if (ts.TotalMilliseconds > MaxDuration)
                    {
                        _clickCount = 0;
                    }
                    else
                    {
                        _clickCount++;
                    }
                }
            }
            else if (e.Action == MotionEventActions.Up)
            {
                float gap = _downX - e.GetX();
                if (gap <= 0 && ((-gap) > 30))
                {
                    if (CurrentSlideIndex == 0)
                    {
                        return true;
                    }

                    int newSlide = CurrentSlideIndex - 1;
                    SetBottomImageList(newSlide);
                    _clickCount = 0;

                    return true;
                }
                else if (gap > 30)
                {
                    if (CurrentSlideIndex + 1 >= SlideList.Count)
                    {
                        return true;
                    }

                    int newSlide = CurrentSlideIndex + 1;
                    SetBottomImageList(newSlide);
                    _clickCount = 0;

                    return true;
                }

                TimeSpan duration = DateTime.Now - _startTime;
                if (duration.TotalMilliseconds > MaxDuration)
                {
                    _clickCount = 0;
                    return true;
                }

                if (_clickCount == 1)
                {
                    _startTime = DateTime.Now;
                }
                else if (_clickCount == 2)
                {
                    DoubleTap();
                    _clickCount = 0;
                }
            }

            return true;
        }

        bool _animationLock = false;
        int _countOfAnimation = 0;
        int _currentAnimation = 0;

        void DoubleTap()
        {
            if (_countOfAnimation <= _currentAnimation)
            {
                return;
            }

            if (_animationLock == true)
            {
                return;
            }

            _animationLock = true;

            SlidePage currentPage = this.SlideList[this.CurrentSlideIndex];
            currentPage.AnimationRemains--;

            string url = string.Format("http://{0}:{1}/nextAnimation", _ip, _port);
            App.CallUrl(url, SetAnimationCompleted);
        }

        void SetAnimationCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            _currentAnimation++;
            _animationLock = false;
        }

        private void SetBottomImageList(int newSlide)
        {
            if (_bottomSlideList == null)
            {
                return;
            }

            BottomItemAdapter adapter = (_bottomSlideList.Adapter as BottomItemAdapter);
            if (adapter == null)
            {
                return;
            }

            ImageView imgView = adapter.GetItem(0) as ImageView;
            float width = imgView.MeasuredWidth;

            _bottomSlideList.ScrollTo((int)width * newSlide);
             adapter.SetCurrentImage(newSlide);
        }
    }
}