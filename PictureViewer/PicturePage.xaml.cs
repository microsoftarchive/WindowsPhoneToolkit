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

namespace PictureViewer
{
    public partial class PicturePage : PhoneApplicationPage
    {
        public PicturePage()
        {
            InitializeComponent();
        }

        protected override void  OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string name = NavigationContext.QueryString["picture"];
            Picture pic = PicturesLoader.GetPicture(name);

            picture1.Source = pic.Bitmap;
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);
        }
    }
}