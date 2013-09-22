using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfficeInterface
{
    public interface IDocumentSnapshot
    {
        string SnapshotText { get; }
        void SetCurrentSlide(int slideNumber);
        void StartShow(int slideNumber);
        void NextAnimation();
    }
}
