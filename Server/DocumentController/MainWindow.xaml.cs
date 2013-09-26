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
using System.Web.Script.Serialization;
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

            this.Closed += new EventHandler(MainWindow_Closed);
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

        void MainWindow_Closed(object sender, EventArgs e)
        {
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

            byte [] fileContents = File.ReadAllBytes(filePath);
            Assembly loaded = Assembly.Load(fileContents);

            object ctlObject = loaded.CreateInstance("DocumentController.ViewerController");
            return ctlObject as IPPTController;
        }

        void btnOpenClicked(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void OpenFile()
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            if (openDlg.ShowDialog() == true)
            {
                string path = openDlg.FileName;
                this.Ready = "Loading...";
                WaitForPriority(DispatcherPriority.Background);

                ProcessDocument(path, _tempPath, _widthForWindowsPhone);
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
                item.Clear();
                if (item.Load(path, newTempPath) == true)
                {
                    int height = (int)(width * _pptSlideHeightRatio);
                    PPTDocument pptDocument = item.ReadAll(width, height);

                    JavaScriptSerializer json = new JavaScriptSerializer();
                    json.MaxJsonLength = Int32.MaxValue;
                    string txt = json.Serialize(pptDocument);
                    if (string.IsNullOrEmpty(txt) == false)
                    {
                        _snapshotForWindowsPhone = txt;
                        _currentController = item;
                        this.Title = string.Format("{0} - {1}KB", _orgTitle, (_snapshotForWindowsPhone.Length / 1024));

                        if (string.IsNullOrEmpty(_snapshotForWindowsPhone) == false)
                        {
                            this.Ready = "Ready";
                            WaitForPriority(DispatcherPriority.Background);
                        }
                        else
                        {
                            this.Ready = "Not Loaded";
                            WaitForPriority(DispatcherPriority.Background);
                        }

                        break;
                    }
                }
            }
        }

        private void CloseAllDocuments()
        {
            foreach (var item in _pptController)
            {
                item.Clear();
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
