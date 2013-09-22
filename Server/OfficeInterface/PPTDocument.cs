using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeInterface
{
    public class PPTDocument
    {
        public PPTDocument()
        {
            _list = new List<PPTPage>();
        }

        List<PPTPage> _list;
        public List<PPTPage> List
        {
            get { return _list; }
            set { _list = value; }
        }

        int _count;
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        int _height;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
