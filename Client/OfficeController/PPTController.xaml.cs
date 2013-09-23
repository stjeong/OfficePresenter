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
        
        bool _animationLock = false;
        
        public App Application
        {
            get { return (App.Current as App); }
        }
        
        public PPTController()
        {
            InitializeComponent();
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
            this.Slides = new System.Collections.ObjectModel.ObservableCollection<SlidePage>();
            int slideIndex = 1;

            foreach (var item in this.Application.Document.List)
            {
                // 이미지 목록에 추가
                byte[] jpegContents = Convert.FromBase64String(item.ImageAsText);
                MemoryStream ms = new MemoryStream(jpegContents);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(ms);
                ms.Dispose();

                SlidePage page = new SlidePage();
                page.Image = bitmapImage;
                page.Memo = item.Note;

                SlideItemData tagData = new SlideItemData();
                tagData.AnimationCount = item.AnimationCount;
                tagData.SlideIndex = slideIndex;

                page.TagData = tagData;

                this.Slides.Add(page);
                slideIndex++;
            }

            StartShow();

            SetSelectedBorder(1);
        }

        void panoramaItem_DoubleTap(object sender, GestureEventArgs e)
        {
            SetNextAnimation();
        }

        void panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            SlideItemData tagData = (e.AddedItems[0] as SlidePage).TagData;

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

            object obj = this.Slides[slideIndex - 1];

            ListBoxItem boxItem =
                imageList.ItemContainerGenerator.ContainerFromItem(obj) as ListBoxItem;

            if (boxItem == null)
            {
                return;
            }

            Border border = this.FindVisualChild<Border>(boxItem);

            if (border == null)
            {
                return;
            }

            border.BorderBrush = new SolidColorBrush(Colors.Yellow);
            border.BorderThickness = new Thickness(1);

            oldBorder = border;

            imageList.ScrollToCenterOfView(obj); 
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

            string url = string.Format("http://{0}:{1}/setSlide/{2}", _ipAddress, _port, number);
            App.CallUrl(url, null);

            _animationLock = false;
        }

        void StartShow()
        {
            SlideItemData tagData = this.Slides[0].TagData;

            countOfAnimation = tagData.AnimationCount;
            currentAnimation = 0;

            string url = string.Format("http://{0}:{1}/startShow", _ipAddress, _port);
            App.CallUrl(url, null);
        }

        void SetNextAnimation()
        {
            if (countOfAnimation <= currentAnimation)
            {
                return;
            }

            if (_animationLock == true)
            {
                return;
            }

            _animationLock = true;
            System.Diagnostics.Debug.WriteLine("AniStart: " + _animationLock);

            string url = string.Format("http://{0}:{1}/nextAnimation", _ipAddress, _port);
            App.CallUrl(url, SetAnimationCompleted);
        }

        void SetAnimationCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            currentAnimation++;
            _animationLock = false;
        }

        // How to: Find DataTemplate-Generated Elements
        // ; http://msdn.microsoft.com/en-us/library/bb613579.aspx
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }
    }
}