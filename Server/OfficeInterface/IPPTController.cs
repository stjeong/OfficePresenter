using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeInterface
{
    public interface IPPTController
    {
        bool Load(string documentPath, string tempPath);
        PPTDocument ReadAll(int width, int height);
        void Clear();

        void SetCurrentSlide(int slideNumber);
        void StartShow(int startSlideNumber);

        void NextAnimation();
    }
}
