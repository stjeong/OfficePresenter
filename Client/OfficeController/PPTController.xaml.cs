using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Text;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using System.IO;
using Microsoft.Phone.Shell;
using System.Runtime.Serialization.Json;
using OfficeInterface;

namespace OfficeController
{
    public partial class PPTController : PhoneApplicationPage
    {
        Border oldBorder = null;
        int countOfAnimation = 0;
        int currentAnimation = 0;

        string _port = string.Empty;
        string _ipAddress = string.Empty;

        public PPTController()
        {
            InitializeComponent();

            this.DataContext = this;

            panorama.SelectionChanged += new EventHandler<SelectionChangedEventArgs>(panorama_SelectionChanged);

        }

        void panItem_DoubleTap(object sender, GestureEventArgs e)
        {
            SetNextAnimation();
        }

        void panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            SlideItemData tagData = (e.AddedItems[0] as PanoramaItem).Tag as SlideItemData;

            int slideIndex = tagData.SlideIndex;
            SetSlide(slideIndex, tagData.AnimationCount);
            SetSelectedBorder(slideIndex);
        }

        void SetSelectedBorder(int slideIndex)
        {
            if (oldBorder != null)
            {
                oldBorder.BorderThickness = new Thickness(0);
            }

            Border border = imageList.Items[slideIndex - 1] as Border;
            border.BorderThickness = new Thickness(1);

            oldBorder = border;

            imageList.ScrollIntoView(border);
        }

        void img_Tap(object sender, GestureEventArgs e)
        {
            SlideItemData tagData = (sender as Image).Tag as SlideItemData;
            
            int slideIndex = tagData.SlideIndex;
            SetSlide(slideIndex, tagData.AnimationCount);
            SetSelectedBorder(slideIndex);
        }

        void SetSlide(int number, int animationCount)
        {
            countOfAnimation = animationCount;
            currentAnimation = 0;

            string url = string.Format("http://{0}:{1}/setSlide/{2}&junk={3}", _ipAddress, _port,
                number, DateTime.Now);

            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.CacheControl] = "no-cache";
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(SetSlideCompleted);
            wc.DownloadStringAsync(new Uri(url));
            _animationLock = false;
        }

        void StartShow()
        {
            PanoramaItem panItem = this.panorama.Items[0] as PanoramaItem;
            SlideItemData tagData = panItem.Tag as SlideItemData;

            countOfAnimation = tagData.AnimationCount;
            currentAnimation = 0;

            string url = string.Format("http://{0}:{1}/startShow&junk={2}", _ipAddress, _port,
                DateTime.Now);

            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.CacheControl] = "no-cache";
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(SetSlideCompleted);
            wc.DownloadStringAsync(new Uri(url));
        }

        public App Application
        {
            get { return (App.Current as App); }
        }

        bool _animationLock = false;
        void SetNextAnimation()
        {
            if (countOfAnimation <= currentAnimation)
            {
                return;
            }

            if (_animationLock == true)
            {
                System.Diagnostics.Debug.WriteLine("Skipped: " + _animationLock);
                return;
            }

            _animationLock = true;
            System.Diagnostics.Debug.WriteLine("AniStart: " + _animationLock);

            string url = string.Format("http://{0}:{1}/nextAnimation&junk={2}", _ipAddress, _port,
                DateTime.Now);

            WebClient wc = new WebClient();
            wc.Headers[HttpRequestHeader.CacheControl] = "no-cache";
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(SetAnimationCompleted);
            wc.DownloadStringAsync(new Uri(url));
        }

        void SetAnimationCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            currentAnimation++;
            _animationLock = false;
            System.Diagnostics.Debug.WriteLine("AniEnd: " + _animationLock);
        }

        void SetSlideCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("Document") == true)
            {
                string data = PhoneApplicationService.Current.State["Document"] as string;

                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PPTDocument));
                    this.Application.Document = serializer.ReadObject(ms) as PPTDocument;
                }
            }

            NavigationContext.QueryString.TryGetValue("ip", out _ipAddress);
            NavigationContext.QueryString.TryGetValue("port", out _port);

            if (this.Application.Document != null)
            {
                LoadDocument();
            }

            base.OnNavigatedTo(e);
        }

        private void LoadDocument()
        {
            int slideIndex = 1;
            foreach (var item in this.Application.Document.List)
            {
                SlideItemData tagData = new SlideItemData();
                tagData.AnimationCount = item.AnimationCount;
                tagData.SlideIndex = slideIndex;

                // 이미지 목록에 추가
                byte[] jpegContents = Convert.FromBase64String(item.ImageAsText);
                Image img = new Image();
                img.Tap += new EventHandler<GestureEventArgs>(img_Tap);

                MemoryStream ms = new MemoryStream(jpegContents);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(ms);
                ms.Dispose();

                img.Source = bitmapImage;
                img.Tag = tagData;

                Border border = new Border();
                border.Child = img;
                border.BorderBrush = new SolidColorBrush(Colors.Yellow);

                if (slideIndex == 1)
                {
                    // 처음 슬라이드를 선택 표시
                    border.BorderThickness = new Thickness(1);
                    oldBorder = border;
                }

                imageList.Items.Add(border);

                // 파노라마 콘트롤에 추가
                PanoramaItem panItem = new PanoramaItem();
                panItem.DoubleTap += new EventHandler<GestureEventArgs>(panItem_DoubleTap);
                panItem.Tag = tagData;
                Grid grid = new Grid();

                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                img = new Image();
                img.Source = bitmapImage;
                Grid.SetRow(img, 0);

                TextBox txt = new TextBox();
                txt.Text = item.Note;
                txt.IsReadOnly = true;
                Grid.SetRow(txt, 1);

                grid.Children.Add(img);
                grid.Children.Add(txt);

                panItem.Content = grid;

                panorama.Items.Add(panItem);

                slideIndex++;
            }

            StartShow();
        }
    }
}