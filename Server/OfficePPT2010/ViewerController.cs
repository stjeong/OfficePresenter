using System;
using System.Collections.Generic;
using System.Text;
using OfficeInterface;
using Microsoft.Office.Interop.PowerPoint;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;

namespace DocumentController
{
    public class ViewerController : IPPTController
    {
        Application _app;
        Presentation _current;
        string _tempPath;

        public bool Load(string documentPath, string tempPath)
        {
            try
            {
                if (_app == null)
                {
                    _app = new Application();
                }

                _app.Visible = Microsoft.Office.Core.MsoTriState.msoTrue;
                var presentation = _app.Presentations;
                _current = presentation.Open2007(documentPath);

                _current.Export(tempPath, "JPG");
                _tempPath = tempPath;
                return true;
            }
            catch
            {
            }

            return false;
        }

        private static int CompareOnlyNumbers(string x, string y)
        {
            string xf = System.IO.Path.GetFileNameWithoutExtension(x);
            string yf = System.IO.Path.GetFileNameWithoutExtension(y);

            int xn = EraseChars(xf);
            int yn = EraseChars(yf);

            return xn.CompareTo(yn);
        }

        private static int EraseChars(string x)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in x)
            {
                if (Char.IsNumber(ch) == true)
                {
                    sb.Append(ch);
                }
            }

            return Int32.Parse(sb.ToString());
        }

        public PPTDocument ReadAll(int width, int height)
        {
            if (Directory.Exists(_tempPath) == false)
            {
                return null;
            }

            int slideNumber = 0;

            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(_tempPath));

            files.Sort(CompareOnlyNumbers);

            PPTDocument document = new PPTDocument();

            for (int i = 0; i < _current.Slides.Count; i ++)
            {
                Slide slide = null;

                try
                {
                    slide = _current.Slides[i];
                }
                catch
                {
                }

                if (slide == null)
                {
                    continue;
                }

                string note = string.Empty;
                int animationCount = 0;

                SlideRange nodePath = null;

                try
                {
                    nodePath = slide.NotesPage;
                }
                catch { }

                if (nodePath != null)
                {
                    foreach (Shape shape in nodePath.Shapes)
                    {
                        PpPlaceholderType currentType = PpPlaceholderType.ppPlaceholderObject;

                        try
                        {
                            currentType = shape.PlaceholderFormat.Type;
                        }
                        catch { }

                        if (currentType == PpPlaceholderType.ppPlaceholderBody)
                        {
                            if (shape.HasTextFrame == Microsoft.Office.Core.MsoTriState.msoTrue)
                            {
                                if (shape.TextFrame.HasText == Microsoft.Office.Core.MsoTriState.msoTrue)
                                {
                                    note += shape.TextFrame.TextRange.Text;
                                }
                            }
                        }

                        AnimationSettings animationSettings = null;

                        try
                        {
                            animationSettings = shape.AnimationSettings;

                            if (animationSettings.Animate == Microsoft.Office.Core.MsoTriState.msoTrue)
                            {
                                animationCount++;
                            }
                        }
                        catch { }
                    }
                }

                try
                {
                    foreach (Effect effect in slide.TimeLine.MainSequence)
                    {
                        if (effect.Timing.TriggerType == MsoAnimTriggerType.msoAnimTriggerOnPageClick)
                        {
                            animationCount++;
                        }
                    }
                }
                catch { }

                PPTPage page = new PPTPage();
                page.Note = note;
                page.ImageAsText = ConvertToImage(files[slideNumber], width, height);
                page.AnimationCount = animationCount;

                slideNumber++;

                document.List.Add(page);
            }

            document.Count = document.List.Count;
            document.Width = width;
            document.Height = height;
            return document;
        }

        private string ConvertToImage(string path, int width, int height)
        {
            try
            {
                using (Image img = Image.FromFile(path))
                {
                    ImageCodecInfo codecInfo = GetEncoderInfo(ImageFormat.Jpeg);

                    using (Image thumbnail = img.GetThumbnailImage(width, height, delegate { return false; }, IntPtr.Zero))
                    {
                        MemoryStream ms = new MemoryStream();

                        EncoderParameters parameters = new EncoderParameters(1);

                        //  Quality: 75
                        parameters.Param[0] = new EncoderParameter(
                            System.Drawing.Imaging.Encoder.Quality, 50L);
                        thumbnail.Save(ms, codecInfo, parameters);

                        // thumbnail.Save(ms, ImageFormat.Png);
                        ms.Position = 0;

                        return Convert.ToBase64String(ms.GetBuffer());
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return Array.Find(
                ImageCodecInfo.GetImageEncoders(),
                (codec) => codec.FormatID == format.Guid);
        }

        public void SetCurrentSlide(int slideNumber)
        {
            try
            {
                _current.SlideShowWindow.View.GotoSlide(slideNumber, Microsoft.Office.Core.MsoTriState.msoTrue);
            }
            catch { }
        }

        public void StartShow(int startSlideNumber)
        {
            object inSlideShow = null;

            try
            {
                inSlideShow = _current.SlideShowWindow;
            }
            catch
            {
            }

            if (inSlideShow == null)
            {
                _current.SlideShowSettings.Run();
                int start = Math.Min(_current.Slides.Count, startSlideNumber);
                _current.SlideShowWindow.View.GotoSlide(start, Microsoft.Office.Core.MsoTriState.msoTrue);
            }
        }

        public void Clear()
        {
            if (_app != null)
            {
                try
                {
                    _app.Quit();
                }
                catch { }

                _app = null;
            }

            _current = null;
        }

        public void NextAnimation()
        {
            _current.SlideShowWindow.Activate();
            _current.SlideShowWindow.View.Next();
        }
    }
}
