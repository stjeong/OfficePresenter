using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace OfficeController
{
    public class PanoramaItemAdapter : BaseAdapter
    {
        PPTController _pptController;
        int _resourceId;

        public PanoramaItemAdapter(Context context, int resourceId)
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

            BorderedImageView imageView = rowView.FindViewById<BorderedImageView>(Resource.Id.panoramaImageRow);
            TextView txtMemo = rowView.FindViewById<TextView>(Resource.Id.txtMemo);

            imageView.SetImageBitmap(_pptController.SlideList[position].Image);
            imageView.Index = position;
            txtMemo.Text = _pptController.SlideList[position].Memo;

            return rowView;
        }
    }
}