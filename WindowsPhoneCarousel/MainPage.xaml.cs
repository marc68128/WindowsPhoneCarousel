using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace WindowsPhoneCarousel
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Carousel.ListImagesCarousel = new List<string>
            {
                "/Images/1.jpg",
                "/Images/2.jpg",
                "/Images/3.jpg",
                "/Images/4.jpg",
            };
          
        }

    }
}