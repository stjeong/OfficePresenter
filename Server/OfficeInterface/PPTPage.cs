using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeInterface
{
    public class PPTPage
    {
        string _imageAsText;
        public string ImageAsText
        {
            get { return _imageAsText; }
            set { _imageAsText = value; }
        }

        string _note;
        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        int _animationCount;
        public int AnimationCount
        {
            get { return _animationCount; }
            set { _animationCount = value; }
        }
    }
}
