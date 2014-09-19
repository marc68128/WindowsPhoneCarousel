using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;

namespace WindowsPhoneCarousel
{
    public partial class UCCarousel : UserControl
    {

        public bool UserFlick { get; set; }
        public UCCarousel()
        {
            InitializeComponent();
            Loaded += OnLoaded;

        }

        public List<Tuple<Image, ImageSettings>> Images { get; set; }


        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ImageSettings.CanvasHeight = LayoutCanvas.ActualHeight;
            ImageSettings.CanvasWidth = LayoutCanvas.ActualWidth;

            LayoutCanvas.Clip = new RectangleGeometry
            {
                Rect = new Rect(new Point(0, 0), new Point(ImageSettings.CanvasWidth, ImageSettings.CanvasHeight))
            };

            Images = new List<Tuple<Image, ImageSettings>>();
            int cmp = 0;
            foreach (var image in ListImagesCarousel)
            {
                var img = new Image
                {
                    Source = new BitmapImage(new Uri(image, UriKind.RelativeOrAbsolute)),
                    Stretch = Stretch.Fill
                };
                var imgSettings = new ImageSettings(-400 + ImageSettings.ImageGap * cmp++);
                img.RefreshSettings(imgSettings);
                Images.Add(new Tuple<Image, ImageSettings>(img, imgSettings));
                LayoutCanvas.Children.Add(img);

            }
        }

        public static DependencyProperty ListImagesCarouselProperty =
            DependencyProperty.Register("ListImagesCarousel", typeof(List<string>), typeof(UCCarousel), new PropertyMetadata(null));

        public List<string> ListImagesCarousel
        {
            get
            {
                return (List<string>)GetValue(ListImagesCarouselProperty);
            }
            set
            {
                SetValue(ListImagesCarouselProperty, value);
            }
        }


        private int cmp = 0;
        private void myGridGestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            foreach (var image in Images)
            {
                image.Item2.ImageShift += e.HorizontalChange;
                image.Item1.RefreshSettings(image.Item2);
            }

            var topImg = (Images.FirstOrDefault(i => i.Item2.IsOnTop));
            if (topImg != null)
            {
                var indexOfTopImage = Images.IndexOf(topImg);
                if (indexOfTopImage >= Images.Count - 1)
                {
                    var first = Images.First();
                    Images.Remove(first);
                    first.Item2.ImageShift = Images.Last().Item2.ImageShift + ImageSettings.ImageGap;
                    Images.Add(first);
                }
                else if (indexOfTopImage == 0)
                {
                    var last = Images.Last();
                    Images.Remove(last);
                    last.Item2.ImageShift = Images.First().Item2.ImageShift - ImageSettings.ImageGap;
                    Images.Insert(0, last);
                }
            }
        }

        private void myGridGestureListener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            if (!UserFlick)
            {
                var topImg = Images.First(i => i.Item2.IsOnTop);
                while (!(Math.Abs(topImg.Item2.ImageShift) < 1))
                {
                    if (topImg.Item2.ImageShift > 0)
                    {
                        Images.ToList().ForEach(i => i.Item2.ImageShift--);
                    }
                    if (topImg.Item2.ImageShift < 0)
                    {
                        Images.ToList().ForEach(i => i.Item2.ImageShift++);
                    }
                    Images.ToList().ForEach(i => i.Item1.RefreshSettings(i.Item2));
                }
            }
            UserFlick = false;
        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            UserFlick = true;
            if (e.HorizontalVelocity > 0)
            {
                var CurrentImg = Images.First(i => i.Item2.IsOnTop);
                var preImage = Images[Images.IndexOf(CurrentImg) - 1];

                while (!(Math.Abs(preImage.Item2.ImageShift) < 1))
                {
                    if (preImage.Item2.ImageShift > 0)
                    {
                        Images.ToList().ForEach(i => i.Item2.ImageShift--);
                    }
                    if (preImage.Item2.ImageShift < 0)
                    {
                        Images.ToList().ForEach(i => i.Item2.ImageShift++);
                    }
                    Images.ToList().ForEach(i => i.Item1.RefreshSettings(i.Item2));
                }
            }
            if (e.HorizontalVelocity < 0)
            {
                var CurrentImg = Images.First(i => i.Item2.IsOnTop);
                var nextImage = Images[Images.IndexOf(CurrentImg) - 1];

                while (!(Math.Abs(nextImage.Item2.ImageShift) < 1))
                {
                    if (nextImage.Item2.ImageShift > 0)
                    {
                        Images.ToList().ForEach(i => i.Item2.ImageShift--);
                    }
                    if (nextImage.Item2.ImageShift < 0)
                    {
                        Images.ToList().ForEach(i => i.Item2.ImageShift++);
                    }
                    Images.ToList().ForEach(i => i.Item1.RefreshSettings(i.Item2));
                }
            }
        }
    }

    public class ImageSettings
    {
        public ImageSettings(double imageShift)
        {
            ImageShift = imageShift;

        }

        public const double HorizontalMargin = 10;
        public const double VerticalMargin = 30;
        public const double ImageGap = 200;
        public static double ImageDefaultHeight { get { return CanvasHeight - HorizontalMargin * 2; } }
        public static double ImageDefaultWidth { get { return CanvasWidth - VerticalMargin * 2; } }

        public static double CanvasHeight { get; set; }
        public static double CanvasWidth { get; set; }

        public bool IsOnTop
        {
            get
            {
                return Math.Abs(ImageShift) < ImageGap / 2;
            }
        }
        public bool IsVisible { get { return ImageHeight != 0 && ImageWidth != 0; } }
        public static double CanvasCenterX { get { return CanvasHeight / 2; } }
        public static double CanvasCenterY { get { return CanvasWidth / 2; } }

        public double ImageHeight { get { return Math.Max(0, ImageDefaultHeight * ((CanvasWidth - Math.Abs(ImageShift)) / CanvasWidth)); } }
        public double ImageWidth { get { return Math.Max(0, ImageDefaultWidth * ((CanvasWidth - Math.Abs(ImageShift)) / CanvasWidth)); } }

        public double CanvasTop { get { return CanvasCenterX - (ImageHeight / 2); } }
        public double CanvasLeft { get { return CanvasCenterY - (ImageWidth / 2) + ImageShift; } }

        public double ImageShift { get; set; }

    }

    public static class ImageExtensions
    {
        public static void RefreshSettings(this Image img, ImageSettings settings)
        {
            Canvas.SetLeft(img, settings.CanvasLeft);
            Canvas.SetTop(img, settings.CanvasTop);
            img.Height = settings.ImageHeight;
            img.Width = settings.ImageWidth;
            img.SetValue(Canvas.ZIndexProperty, settings.IsOnTop ? 10 : 0);
        }
    }
}
