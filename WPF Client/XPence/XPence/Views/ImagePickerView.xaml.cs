using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace XPence.Views
{
    /// <summary>
    /// Interaction logic for ImagePickerView.xaml
    /// </summary>
    public partial class ImagePickerView
    {
        public ImagePickerView()
        {
            InitializeComponent();
        }

        private void FileOpenButtonClick(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box 
            var dlg = new Microsoft.Win32.OpenFileDialog
                          {FileName = "", DefaultExt = ".jpg", Filter = "Text documents (.jpg)|*.jpg"};

            // Show open file dialog box 
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                var filename = dlg.FileName;
                croppy.Source = new BitmapImage(new Uri(filename));
            }
        }
    }
}
