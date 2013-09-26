using System;
using System.Collections.Generic;
using System.Text;
using OfficeInterface;

namespace DocumentController
{
    public class ViewerController : IPPTController
    {
        public bool Load(string documentPath, string tempPath)
        {
            return true;
        }

        public PPTDocument ReadAll(int width, int height)
        {
            return null;
        }

        public void Clear()
        {
        }

        public void SetCurrentSlide(int slideNumber)
        {
        }

        public void StartShow(int startSlideNumber)
        {
        }

        public void NextAnimation()
        {
        }
    }
}
