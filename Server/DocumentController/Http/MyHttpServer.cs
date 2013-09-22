using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bend.Util;
using System.IO;
using OfficeInterface;

namespace DocumentController.Http
{
    public class MyHttpServer : HttpServer
    {
        IDocumentSnapshot _document;

        public MyHttpServer(IDocumentSnapshot document, int port)
            : base(port)
        {
            _document = document;
        }

        public override void handleGETRequest(HttpProcessor p)
        {
            string urlText = p.http_url;

            int pos = urlText.IndexOf("&");
            if (pos != -1)
            {
                urlText = urlText.Substring(0, pos);
            }

            if (urlText.EndsWith("/getSnapshot") == true)
            {
                p.writeSuccess("application/json");
                p.outputStream.Write(_document.SnapshotText);
            }
            else if (urlText.Contains("/setSlide/") == true)
            {
                string txt = urlText.Substring(urlText.LastIndexOf('/') + 1);

                int slide;
                if (Int32.TryParse(txt, out slide) == true)
                {
                    _document.SetCurrentSlide(slide);
                }

                p.writeSuccess("text/html");
                p.outputStream.Write("OK");
            }
            else if (urlText.Contains("/startShow/") == true)
            {
                string txt = urlText.Substring(urlText.LastIndexOf('/') + 1);

                int slide;
                if (Int32.TryParse(txt, out slide) == true)
                {
                    _document.StartShow(slide);
                }

                p.writeSuccess("text/html");
                p.outputStream.Write("OK");
            }
            else if (urlText.Contains("/startShow") == true)
            {
                _document.StartShow(1);
                p.writeSuccess("text/html");
                p.outputStream.Write("OK");
            }
            else if (urlText.Contains("/nextAnimation") == true)
            {
                _document.NextAnimation();
                p.writeSuccess("text/html");
                p.outputStream.Write("OK");
            }
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();

            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
        }
    }
}
