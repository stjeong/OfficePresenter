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
    public class BottomHorizontalListView : Cheesebaron.HorizontalListView.HorizontalListView
    {
        public BottomHorizontalListView(Context ctx)
            : base(ctx)
        {
        }
        
        public BottomHorizontalListView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
        }

        public BottomHorizontalListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }
    }
}