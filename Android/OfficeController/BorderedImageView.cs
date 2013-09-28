using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace OfficeController
{
    // http://stackoverflow.com/questions/3496269/how-to-put-a-border-around-an-android-textview
    public class BorderedImageView : ImageView
    {
        private Paint paint = new Paint();
        public const int BORDER_TOP = 0x00000001;
        public const int BORDER_RIGHT = 0x00000002;
        public const int BORDER_BOTTOM = 0x00000004;
        public const int BORDER_LEFT = 0x00000008;

        public bool Border { get; set; }
        public int Index { get; set; }

        public BorderedImageView(Context context)
            : base(context)
        {
            init();
        }

        public BorderedImageView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            init();
        }

        public BorderedImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            init();
        }

        private void init()
        {
            paint.SetStyle(Paint.Style.Stroke);
            paint.Color = Color.Yellow;
            paint.StrokeWidth = 2;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (Border == false) return;

            float width = this.Width - (paint.StrokeWidth);
            float height = this.Height - (paint.StrokeWidth);

            canvas.DrawLine(0, 0, width, 0, paint);
            canvas.DrawLine(width, 0, width, height, paint);
            canvas.DrawLine(0, height, width, height, paint);
            canvas.DrawLine(0, 0, 0, height, paint);
        }
    }
}