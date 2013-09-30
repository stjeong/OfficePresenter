using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace OfficeController
{
    public class TimeoutContext
    {
        public int Timeout { get; set; }
        public bool Completed { get; set; }
        public object Tag { get; set; }
        public bool Connected { get; set; }

        public TimeoutContext()
        {
        }

        public void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.BytesReceived != 0)
            {
                this.Connected = true;
            }
        }
    }
}
