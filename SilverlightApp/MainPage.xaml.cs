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
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            listBox1.ItemsSource = Picture.GetPictures(6);
            listBox2.ItemsSource = Picture.GetPictures(8);
        }

        private void ShowPicture(string name)
        {
            Uri uri = new Uri(string.Format("/PicturePage.xaml?picture={0}", name), UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("you clicked !", "yeah", MessageBoxButton.OK);
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
    }
}
