using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace OfficeController
{
    public class BottomItemAdapter : BaseAdapter, View.IOnTouchListener
    {
        PPTController _pptController;
        int _resourceId;
        BorderedImageView _oldSelected;

        Dictionary<int, BorderedImageView> _imageList;

        public BottomItemAdapter(Context context, int resourceId)
        {
            _pptController = context as PPTController;
            _resourceId = resourceId;

            _imageList = new Dictionary<int, BorderedImageView>();
        }

        public override int Count { get { return _pptController.SlideList.Count; } }

        public override Java.Lang.Object GetItem(int position)
        {
            if (_imageList.Count < position + 1)
            {
                return null;
            }

            return _imageList[position];
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        bool _first = true;
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflator = _pptController.GetSystemService(Context.LayoutInflaterService)
                as LayoutInflater;

            View rowView = inflator.Inflate(_resourceId, parent, false);
            BorderedImageView imageView = rowView.FindViewById<BorderedImageView>(Resource.Id.bottomRowImage);
            imageView.SetImageBitmap(_pptController.SlideList[position].Image);

            imageView.Index = position;
            imageView.SetOnTouchListener(this);

            if (_imageList.ContainsKey(position) == true)
            {
                imageView.Border = _imageList[position].Border;
            }

            if (_first == true && position == 0)
            {
                _first = false;
                _oldSelected = imageView;
                _oldSelected.Border = true;
            }

            _imageList[position] = imageView;

            return rowView;
        }

        void imageView_Click(object sender, EventArgs e)
        {
            BorderedImageView ctx = (sender as BorderedImageView);
            if (ctx == null)
            {
                return;
            }

            foreach (var item in _imageList.Values)
            {
                if (item.Index == ctx.Index)
                {
                    item.Border = true;
                }
                else
                {
                    item.Border = false;
                }

                item.Invalidate();
            }

            _pptController.SetSlide(ctx.Index);
        }

        internal void SetCurrentImage(int newSlide)
        {
            BorderedImageView current = GetItem(newSlide) as BorderedImageView;
            if (current == null)
            {
                return;
            }

            imageView_Click(current, EventArgs.Empty);
        }

        float _rawX;

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                _rawX = e.RawX;
            }
            else if (e.Action == MotionEventActions.Up)
            {
                // click?
                if (Math.Abs(_rawX - e.RawX) < 10)
                {
                    imageView_Click(v, EventArgs.Empty);
                    return false;
                }
            }

            return true;
        }
    }
}