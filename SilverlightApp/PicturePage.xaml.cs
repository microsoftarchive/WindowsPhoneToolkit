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
using System.Windows.Navigation;

namespace SilverlightApp
{
    public partial class PicturePage : Page
    {
        public PicturePage()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string name = NavigationContext.QueryString["picture"];
            Picture pic = Picture.Get(name);

            picture1.Source = pic.Bitmap;
        }

    }
}
