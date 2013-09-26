using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.NetworkInformation;
using System.IO;
using OfficeInterface;
using System.Reflection;
using DocumentController.Http;
using System.Threading;
using Microsoft.Win32;
using System.Windows.Threading;

namespace DocumentController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDocumentSnapshot
    {
        List<IPPTController> _pptController;
        string _tempPath;
        float _pptSlideHeightRatio;

        int _widthForWindowsPhone = 480;

        string _snapshotForWindowsPhone = string.Empty;
        public string SnapshotText
        {
            get { return _snapshotForWindowsPhone; }
        }

        MyHttpServer _listener;
        Thread _httpThread;
        IPPTController _currentController;

        string _orgTitle;

        public MainWindow()
        {
            InitializeComponent();

            this.Ready = "Not Loaded";

            _orgTitle = this.Title;
            _pptSlideHeightRatio = 391f / 523f;
            this.Port = 5022;

            this.DataContext = this;

            this.IPList = new System.Collections.ObjectModel.ObservableCollection<string>();
            PopulateIPList(this.IPList);

            this._pptController = new List<IPPTController>();
            LoadPlugins();

            _tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "OfficePresenter");
            SetupHttpServer();
        }

        public void StartShow(int slideNumber)
        {
            this.Dispatcher.BeginInvoke(
              new ThreadStart(
                  () =>
                  {
                      _currentController.StartShow(slideNumber);
                  }), null
          );
        }

        public void SetCurrentSlide(int slideNumber)
        {
            this.Dispatcher.BeginInvoke(
                new ThreadStart(
                    () =>
                    {
                        _currentController.SetCurrentSlide(slideNumber);
                    }), null
            );
        }

        public void NextAnimation()
        {
            Thread.Sleep(500);
            this.Dispatcher.Invoke(
                new ThreadStart(
                    () =>
                    {
                        _currentController.NextAnimation();
                    }), null
            );
        }

        private void SetupHttpServer()
        {
            ReleaseSocketRelatedResource();

            _listener = new MyHttpServer(this, this.Port);
            _httpThread = new Thread(new ThreadStart(_listener.listen));
            _httpThread.IsBackground = true;
            _httpThread.Start();
        }

        private void DeleteOldSnapshot()
        {
            if (Directory.Exists(_tempPath) == true)
            {
                Directory.Delete(_tempPath, true);
            }
        }

        private void LoadPlugins()
        {
            _pptController.Add(LoadAssembly("OfficePPT2010") as IPPTController);
            _pptController.Add(LoadAssembly("OfficePPT2007") as IPPTController);
        }

        private IPPTController LoadAssembly(string fileName)
        {
            string folder = System.IO.Path.GetDirectoryName(typeof(MainWindow).Assembly.Location);
            string filePath = System.IO.Path.Combine(folder, fileName + ".dll");

            byte[] fileContents = File.ReadAllBytes(filePath);
            Assembly loaded = Assembly.Load(fileContents);

            return loaded.CreateInstance("DocumentController.ViewerController") as IPPTController;
        }

        private void btnOpenClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            if (openDlg.ShowDialog() == true)
            {
                string path = openDlg.FileName;

                EnableControls(false);

                WaitForPriority(DispatcherPriority.Background);

                ThreadPool.QueueUserWorkItem(
                    (obj) =>
                    {
                        try
                        {
                            ProcessDocument(path, _tempPath, _widthForWindowsPhone);
                        }
                        catch
                        {
                        }

                        Dispatcher.BeginInvoke(
                            (ThreadStart)(() =>
                            {
                                this.Title = string.Format("{0} - {1}KB", _orgTitle, (_snapshotForWindowsPhone.Length / 1024));
                                EnableControls(true);
                            }));
                    }, null);
            }
        }

        private void EnableControls(bool enable)
        {
            if (enable == false)
            {
                this.Ready = "Loading...";
                contentPanel.IsEnabled = false;
                _snapshotForWindowsPhone = string.Empty;

                // http://fragiledevelopment.wordpress.com/2009/11/13/rotating-an-image-with-wpf-in-xaml/
                waitImage.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                contentPanel.IsEnabled = true;

                waitImage.IsEnabled = false;
                waitImage.Visibility = System.Windows.Visibility.Hidden;
                if (string.IsNullOrEmpty(_snapshotForWindowsPhone) == false)
                {
                    this.Ready = "Ready";
                    WaitForPriority(DispatcherPriority.Background);
                }
                else
                {
                    this.Ready = "Not Loaded";
                    this.FilePath = string.Empty;
                    WaitForPriority(DispatcherPriority.Background);
                }
            }
        }

        private void ProcessDocument(string path, string tempPath, int width)
        {
            if (File.Exists(path) == false)
            {
                return;
            }

            this.FilePath = path;

            _currentController = null;

            DeleteOldSnapshot();

            string newTempPath = System.IO.Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(newTempPath);

            foreach (var item in _pptController)
            {
                if (item.Load(path, newTempPath) == true)
                {
                    int height = (int)(width * _pptSlideHeightRatio);
                    PPTDocument pptDocument = item.ReadAll(width, height);

                    string txt = Newtonsoft.Json.JsonConvert.SerializeObject(pptDocument);
                    if (string.IsNullOrEmpty(txt) == false)
                    {
                        _snapshotForWindowsPhone = txt;
                        _currentController = item;

                        break;
                    }
                }
            }
        }

        private void PopulateIPList(System.Collections.ObjectModel.ObservableCollection<string> list)
        {
            foreach (var item in GetInetAddress(System.Net.Sockets.AddressFamily.InterNetwork))
            {
                list.Add(item.ToString());
            }
        }

        public static IPAddress[] GetInetAddress(System.Net.Sockets.AddressFamily family)
        {
            List<IPAddress> ipAddresses = new List<IPAddress>();

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface nic in nics)
            {
                foreach (UnicastIPAddressInformation uni in nic.GetIPProperties().UnicastAddresses)
                {
                    if (uni.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (System.Net.IPAddress.Loopback.ToString() == uni.Address.ToString())
                        {
                            continue;
                        }

                        ipAddresses.Add(uni.Address);
                    }
                }
            }

            return ipAddresses.ToArray();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            ReleaseSocketRelatedResource();
            CloseAllDocuments();
        }

        private void ReleaseSocketRelatedResource()
        {
            try
            {
                if (_listener != null)
                {
                    _listener.Dispose();
                }
            }
            catch { }
            _listener = null;

            try
            {
                if (_httpThread != null)
                {
                    _httpThread.Abort();
                }
            }
            catch { }
            _httpThread = null;
        }

        private void CloseAllDocuments()
        {
            foreach (var item in _pptController)
            {
                item.Clear();
            }
        }

        internal static void WaitForPriority(DispatcherPriority priority)
        {
            DispatcherFrame frame = new DispatcherFrame();
            DispatcherOperation dispatcherOperation =
                Dispatcher.CurrentDispatcher.BeginInvoke(priority,
                    new DispatcherOperationCallback(ExitFrameOperation), frame);
            Dispatcher.PushFrame(frame);
            if (dispatcherOperation.Status != DispatcherOperationStatus.Completed)
            {
                dispatcherOperation.Abort();
            }
        }

        private static object ExitFrameOperation(object obj)
        {
            ((DispatcherFrame)obj).Continue = false;
            return null;
        }

    }
}
