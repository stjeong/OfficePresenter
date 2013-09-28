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
    public class BottomItemAdapter : BaseAdapter
    {
        PPTController _pptController;
        int _resourceId;
        BorderedImageView _oldSelected;

        public BottomItemAdapter(Context context, int resourceId)
        {
            _pptController = context as PPTController;
            _resourceId = resourceId;
        }

        public override int Count { get { return _pptController.SlideList.Count; } }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater inflator = _pptController.GetSystemService(Context.LayoutInflaterService)
                as LayoutInflater;

            View rowView = inflator.Inflate(_resourceId, parent, false);
            BorderedImageView imageView = rowView.FindViewById<BorderedImageView>(Resource.Id.bottomRowImage);
            imageView.SetImageBitmap(_pptController.SlideList[position].Image);

            imageView.Click += imageView_Click;
            imageView.Index = position;

            if (position == 0)
            {
                _oldSelected = imageView;
                _oldSelected.Border = true;
            }

            return rowView;
        }

        void imageView_Click(object sender, EventArgs e)
        {
            BorderedImageView ctx = (sender as BorderedImageView);
            if (ctx == null)
            {
                return;
            }

            if (_oldSelected != null)
            {
                _oldSelected.Border = false;
                _oldSelected.Invalidate();
            }

            ctx.Border = true;
            ctx.Invalidate();

            _oldSelected = ctx;

            _pptController.SetSlide(ctx.Index);
        }
    }
}