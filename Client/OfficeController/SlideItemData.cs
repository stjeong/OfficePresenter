using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OfficeController
{
    public class SlideItemData
    {
        int _slideIndex;
        public int SlideIndex
        {
            get { return _slideIndex; }
            set { _slideIndex = value; }
        }

        int _animationCount;
        public int AnimationCount
        {
            get { return _animationCount; }
            set { _animationCount = value; }
        }
    }
}
