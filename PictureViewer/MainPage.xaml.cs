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
using Phone.Controls.Samples;
using System.ComponentModel;

namespace PictureViewer
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ShowPicture(string name)
        {
            Uri uri = new Uri(string.Format("/PicturePage.xaml?picture={0}", name), UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Picture pic = (Picture)e.AddedItems[0];
                ShowPicture(pic.Name);

                // reset selection
                ((ListBox)sender).SelectedIndex = -1;
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            // allow the back key
            e.Cancel = false;

            // go back further, skipping past the splash screen
            NavigationService.GoBack();
        }
    }
}