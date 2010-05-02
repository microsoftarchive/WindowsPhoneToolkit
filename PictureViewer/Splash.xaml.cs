using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;

namespace PictureViewer
{
    public partial class Splash : PhoneApplicationPage
    {
        private readonly Uri uriLightLogo = new Uri("/PictureViewer;component/Resources/bingLogo.png", UriKind.Relative);
        private readonly Uri uriDarkLogo = new Uri("/PictureViewer;component/Resources/bingLogo_reverse.png", UriKind.Relative);

        public Splash()
        {
            InitializeComponent();

            // change logo based on light/dark theme
            ResourceDictionary themes = Application.Current.Resources;
            if (themes.Contains("PhoneBackgroundColor"))
            {
                Color color = (Color)themes["PhoneBackgroundColor"];
                imgLogo.Source = new BitmapImage(
                    color == Colors.White ? uriLightLogo : uriDarkLogo);
            }

            // initialize
            PicturesLoader.LoadProgress += Picture_LoadProgress;
            PicturesLoader.LoadCompleted += Picture_LoadCompleted;
            PicturesLoader.LoadPictures();
        }

        void Picture_LoadProgress(object sender, PicturesLoaderProgressEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                txtLoading.Text = string.Format("Loading from Bing.com ({0}/{1})...", e.Num, e.Max);
            });
        }

        void Picture_LoadCompleted(object sender, PicturesLoaderCompletedEventArgs e)
        {
            PicturesLoader.LoadProgress -= Picture_LoadProgress;
            PicturesLoader.LoadCompleted -= Picture_LoadCompleted;
            this.Dispatcher.BeginInvoke(() =>
            {
                Uri uri = new Uri("/MainPage.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            });
        }
    }
}