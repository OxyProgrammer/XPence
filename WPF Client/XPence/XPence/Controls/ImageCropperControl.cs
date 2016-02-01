/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XPence.Infrastructure.Utility;

namespace XPence.Controls
{
    public class ImageCropperControl : Image
    {
        private static readonly DependencyPropertyKey OutputImagePropertyKey = DependencyProperty.RegisterReadOnly("OutputImage", typeof(ImageSource), typeof(ImageCropperControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnReadOnlyPropChanged)));

        public static readonly DependencyProperty OutputImageProperty = OutputImagePropertyKey.DependencyProperty;

        public ImageSource OutputImage
        {
            get { return (ImageSource)GetValue(OutputImageProperty); }
            protected set { SetValue(OutputImagePropertyKey, value); }
        }

        #region Private static event handlers.

        private static void OnReadOnlyPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ImageCropperControl;
            if (null != control)
            {
                control.OnReadOnlyPropChanged(e);
            }
        }

        private static void SourcePropertyChanged(DependencyObject dependencyObject,
                                                  DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as ImageCropperControl;
            if (null == control) //Useless check but I am obsessed.
                return;
            //if (e.NewValue == null)
            //{
            //    control.SetNullImage();
            //}
            control.AddCropToElement();
            control.RefreshCropImage();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Intializes an instance of <see cref="ImageCropperControl"/>.
        /// </summary>
        public ImageCropperControl()
        {
            PreviewDragEnter += FileShowImageCropperControlPreviewDragEnter;
            PreviewDragOver += FileShowImageCropperControlPreviewDragEnter;
            PreviewDrop += FileImageCropperControlPreviewDrop;
            AllowDrop = true;
            Loaded += (o, e) =>
                          {
                              AddCropToElement();
                              RefreshCropImage();
                          };
            //SetNullImage();
        }

        /// <summary>
        /// static constructor to hook up dependency property overrides etc.
        /// </summary>
        static ImageCropperControl()
        {
            SourceProperty.OverrideMetadata(typeof (ImageCropperControl),
                                            new FrameworkPropertyMetadata(null, SourcePropertyChanged));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void OnReadOnlyPropChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Removes crop from the image.
        /// </summary>
        private void RemoveCropFromCur()
        {
            var aly = AdornerLayer.GetAdornerLayer(_felCur);
            aly.Remove(_clp);
        }

        /// <summary>
        /// Add crop to the new image.
        /// </summary>
        private void AddCropToElement()
        {
            if (_felCur != null)
            {
                RemoveCropFromCur();
            }
            var rcInterior = new Rect(
                ActualWidth*0.2,
                ActualHeight*0.2,
                ActualWidth*0.6,
                ActualHeight*0.6);
            var aly = AdornerLayer.GetAdornerLayer(this);
            _clp = new CroppingAdorner(this, rcInterior);
            var clr = Colors.Black;
            clr.A = 110;
            _clp.Fill = new SolidColorBrush(clr);
            if (aly != null)
            {
                aly.Add(_clp);
                OutputImage = _clp.BpsCrop();
                _clp.CropChanged += CropChanged;
                _felCur = this;
            }
        }

        /// <summary>
        /// Refreshes the cropped image output when the crop changes.
        /// </summary>
        private void RefreshCropImage()
        {
            if (_clp == null) 
                return;
            var rc = _clp.ClippingRectangle;
            var bSource = _clp.BpsCrop();
            OutputImage = CreateResizedImage(bSource, OutputImageWidth, OutputImageHeight);
        }

        /// <summary>
        /// Fires whent he crop is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="rea"></param>
        private void CropChanged(Object sender, RoutedEventArgs rea)
        {
            RefreshCropImage();
        }

        /// <summary>
        /// Creates a new ImageSource with the specified width/height
        /// </summary>
        /// <param name="source">Source image to resize</param>
        /// <param name="width">Width of resized image</param>
        /// <param name="height">Height of resized image</param>
        /// <returns>Resized image</returns>
        private ImageSource CreateResizedImage(ImageSource source, int width, int height)
        {
            // Target Rect for the resize operation
            var rect = new Rect(0, 0, width, height);

            // Create a DrawingVisual/Context to render with
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(source, rect);
            }

            // Use RenderTargetBitmap to resize the original image
            var resizedImage = new RenderTargetBitmap(
                (int) rect.Width, (int) rect.Height, // Resized dimensions
                96, 96, // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);
            // Return the resized image
            return resizedImage;
        }

        /// <summary>
        /// Sets null image for the control.
        /// </summary>
        private void SetNullImage()
        {
            BitmapImage image = null;
            try
            {
                image = Application.Current.TryFindResource("NullImageSource") as BitmapImage;
            }
            catch (Exception ex)
            {
                //This control work if it lands here.
                //Need to device a better way.For the time being let it crash.
                LogUtil.LogError("", "SourcePropertyChanged", ex);
                throw;
            }
            Source = image;
        }

        #endregion

        #region Drag Drop Code

        /// <summary>
        /// Preview Drag Enter event handler.
        /// Place to check if the dragged files should be allowed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void FileShowImageCropperControlPreviewDragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (var filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                    var info = new FileInfo(filename);
                    if (info.Extension.ToLower() == ".jpg" || info.Extension.ToLower() == ".png")
                        continue;
                    isCorrect = false;
                    break;
                }
            }
            e.Effects = isCorrect ? DragDropEffects.All : DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Handles the drop event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileImageCropperControlPreviewDrop(object sender, DragEventArgs e)
        {
            var droppedFilenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            if (droppedFilenames != null && droppedFilenames.Length > 0)
            {
                ImageSource source = new BitmapImage(new Uri(droppedFilenames[0]));
                Source = source;
            }
            e.Handled = true;
        }

        #endregion

        #region Member Variables

        private const int OutputImageHeight = 75;
        private const int OutputImageWidth = 75;
        private CroppingAdorner _clp;
        private FrameworkElement _felCur = null;

        #endregion

    }
}
