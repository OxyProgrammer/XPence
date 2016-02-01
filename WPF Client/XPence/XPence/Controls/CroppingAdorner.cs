/********************************************************************************
 * Has been acquired as it is from 
 * http://www.codeproject.com/Articles/23158/A-Photoshop-like-Cropping-Adorner-for-WPF
 * *******************************************************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point = System.Drawing.Point;

namespace XPence.Controls
{
    public class CroppingAdorner : Adorner
    {
        #region Private variables
        // Width of the thumbs.  I know these really aren't "pixels", but px
        // is still a good mnemonic.
        private const int _cpxThumbWidth = 6;

        // PuncturedRect to hold the "Cropping" portion of the adorner
        private PuncturedRect _prCropMask;

        // Canvas to hold the thumbs so they can be moved in response to the user
        private Canvas _cnvThumbs;

        // Cropping adorner uses Thumbs for visual elements.  
        // The Thumbs have built-in mouse input handling.
        private CropThumb _crtTopLeft, _crtTopRight, _crtBottomLeft, _crtBottomRight;
        private CropThumb _crtTop, _crtLeft, _crtBottom, _crtRight;

        // To store and manage the adorner's visual children.
        private VisualCollection _vc;

        // DPI for screen
        private static double s_dpiX, s_dpiY;
        #endregion

        #region Properties
        public Rect ClippingRectangle
        {
            get
            {
                return _prCropMask.RectInterior;
            }
        }
        #endregion

        #region Routed Events
        public static readonly RoutedEvent CropChangedEvent = EventManager.RegisterRoutedEvent(
            "CropChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(CroppingAdorner));

        public event RoutedEventHandler CropChanged
        {
            add
            {
                AddHandler(CropChangedEvent, value);
            }
            remove
            {
                RemoveHandler(CropChangedEvent, value);
            }
        }
        #endregion

        #region Dependency Properties
        static public DependencyProperty FillProperty = Shape.FillProperty.AddOwner(typeof(CroppingAdorner));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        private static void FillPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            CroppingAdorner crp = d as CroppingAdorner;

            if (crp != null)
            {
                crp._prCropMask.Fill = (Brush)args.NewValue;
            }
        }
        #endregion

        #region Constructor
        static CroppingAdorner()
        {
            Color clr = Colors.Red;
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd((IntPtr)0);

            s_dpiX = g.DpiX;
            s_dpiY = g.DpiY;
            clr.A = 80;
            FillProperty.OverrideMetadata(typeof(CroppingAdorner),
                new PropertyMetadata(
                    new SolidColorBrush(clr),
                    new PropertyChangedCallback(FillPropChanged)));
        }

        public CroppingAdorner(UIElement adornedElement, Rect rcInit)
            : base(adornedElement)
        {
            _vc = new VisualCollection(this);
            _prCropMask = new PuncturedRect();
            _prCropMask.IsHitTestVisible = false;
            _prCropMask.RectInterior = rcInit;
            _prCropMask.Fill = Fill;
            _vc.Add(_prCropMask);
            _cnvThumbs = new Canvas();
            _cnvThumbs.HorizontalAlignment = HorizontalAlignment.Stretch;
            _cnvThumbs.VerticalAlignment = VerticalAlignment.Stretch;

            _vc.Add(_cnvThumbs);
            BuildCorner(ref _crtTop, Cursors.SizeNS);
            BuildCorner(ref _crtBottom, Cursors.SizeNS);
            BuildCorner(ref _crtLeft, Cursors.SizeWE);
            BuildCorner(ref _crtRight, Cursors.SizeWE);
            BuildCorner(ref _crtTopLeft, Cursors.SizeNWSE);
            BuildCorner(ref _crtTopRight, Cursors.SizeNESW);
            BuildCorner(ref _crtBottomLeft, Cursors.SizeNESW);
            BuildCorner(ref _crtBottomRight, Cursors.SizeNWSE);

            // Add handlers for Cropping.
            _crtBottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            _crtBottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            _crtTopLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            _crtTopRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
            _crtTop.DragDelta += new DragDeltaEventHandler(HandleTop);
            _crtBottom.DragDelta += new DragDeltaEventHandler(HandleBottom);
            _crtRight.DragDelta += new DragDeltaEventHandler(HandleRight);
            _crtLeft.DragDelta += new DragDeltaEventHandler(HandleLeft);

            // We have to keep the clipping interior withing the bounds of the adorned element
            // so we have to track it's size to guarantee that...
            FrameworkElement fel = adornedElement as FrameworkElement;

            if (fel != null)
            {
                fel.SizeChanged += new SizeChangedEventHandler(AdornedElement_SizeChanged);
            }
        }

        #endregion

        #region Thumb handlers
        // Generic handler for Cropping
        private void HandleThumb(
            double drcL,
            double drcT,
            double drcW,
            double drcH,
            double dx,
            double dy)
        {
            Rect rcInterior = _prCropMask.RectInterior;

            if (rcInterior.Width + drcW * dx < 0)
            {
                dx = -rcInterior.Width / drcW;
            }

            if (rcInterior.Height + drcH * dy < 0)
            {
                dy = -rcInterior.Height / drcH;
            }

            rcInterior = new Rect(
                rcInterior.Left + drcL * dx,
                rcInterior.Top + drcT * dy,
                rcInterior.Width + drcW * dx,
                rcInterior.Height + drcH * dy);

            _prCropMask.RectInterior = rcInterior;
            SetThumbs(_prCropMask.RectInterior);
            RaiseEvent(new RoutedEventArgs(CropChangedEvent, this));
        }

        // Handler for Cropping from the bottom-left.
        private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            if (sender is CropThumb)
            {
                HandleThumb(
                    1, 0, -1, 1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the bottom-right.
        private void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            if (sender is CropThumb)
            {
                HandleThumb(
                    0, 0, 1, 1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the top-right.
        private void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            if (sender is CropThumb)
            {
                HandleThumb(
                    0, 1, 1, -1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the top-left.
        private void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            if (sender is CropThumb)
            {
                HandleThumb(
                    1, 1, -1, -1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the top.
        private void HandleTop(object sender, DragDeltaEventArgs args)
        {
            if (sender is CropThumb)
            {
                HandleThumb(
                    0, 1, 0, -1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the left.
        private void HandleLeft(object sender, DragDeltaEventArgs args)
        {
            if (sender is CropThumb)
            {
                HandleThumb(
                    1, 0, -1, 0,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the right.
        private void HandleRight(object sender, DragDeltaEventArgs args)
        {
            if (sender is CropThumb)
            {
                HandleThumb(
                    0, 0, 1, 0,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }

        // Handler for Cropping from the bottom.
        private void HandleBottom(object sender, DragDeltaEventArgs args)
        {
            if (sender is CropThumb)
            {
                HandleThumb(
                    0, 0, 0, 1,
                    args.HorizontalChange,
                    args.VerticalChange);
            }
        }
        #endregion

        #region Other handlers
        private void AdornedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement fel = sender as FrameworkElement;
            Rect rcInterior = _prCropMask.RectInterior;
            bool fFixupRequired = false;
            double
                intLeft = rcInterior.Left,
                intTop = rcInterior.Top,
                intWidth = rcInterior.Width,
                intHeight = rcInterior.Height;

            if (rcInterior.Left > fel.RenderSize.Width)
            {
                intLeft = fel.RenderSize.Width;
                intWidth = 0;
                fFixupRequired = true;
            }

            if (rcInterior.Top > fel.RenderSize.Height)
            {
                intTop = fel.RenderSize.Height;
                intHeight = 0;
                fFixupRequired = true;
            }

            if (rcInterior.Right > fel.RenderSize.Width)
            {
                intWidth = Math.Max(0, fel.RenderSize.Width - intLeft);
                fFixupRequired = true;
            }

            if (rcInterior.Bottom > fel.RenderSize.Height)
            {
                intHeight = Math.Max(0, fel.RenderSize.Height - intTop);
                fFixupRequired = true;
            }
            if (fFixupRequired)
            {
                _prCropMask.RectInterior = new Rect(intLeft, intTop, intWidth, intHeight);
            }
        }
        #endregion

        #region Arranging/positioning
        private void SetThumbs(Rect rc)
        {
            _crtBottomRight.SetPos(rc.Right, rc.Bottom);
            _crtTopLeft.SetPos(rc.Left, rc.Top);
            _crtTopRight.SetPos(rc.Right, rc.Top);
            _crtBottomLeft.SetPos(rc.Left, rc.Bottom);
            _crtTop.SetPos(rc.Left + rc.Width / 2, rc.Top);
            _crtBottom.SetPos(rc.Left + rc.Width / 2, rc.Bottom);
            _crtLeft.SetPos(rc.Left, rc.Top + rc.Height / 2);
            _crtRight.SetPos(rc.Right, rc.Top + rc.Height / 2);
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rcExterior = new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);
            _prCropMask.RectExterior = rcExterior;
            Rect rcInterior = _prCropMask.RectInterior;
            _prCropMask.Arrange(rcExterior);

            SetThumbs(rcInterior);
            _cnvThumbs.Arrange(rcExterior);
            return finalSize;
        }
        #endregion

        #region Public interface
        public BitmapSource BpsCrop()
        {
            Thickness margin = AdornerMargin();
            Rect rcInterior = _prCropMask.RectInterior;

            Point pxFromSize = UnitsToPx(rcInterior.Width, rcInterior.Height);

            // It appears that CroppedBitmap indexes from the upper left of the margin whereas RenderTargetBitmap renders the
            // control exclusive of the margin.  Hence our need to take the margins into account here...

            Point pxFromPos = UnitsToPx(rcInterior.Left + margin.Left, rcInterior.Top + margin.Top);
            Point pxWhole = UnitsToPx(AdornedElement.RenderSize.Width + margin.Left, AdornedElement.RenderSize.Height + margin.Left);
            pxFromSize.X = Math.Max(Math.Min(pxWhole.X - pxFromPos.X, pxFromSize.X), 0);
            pxFromSize.Y = Math.Max(Math.Min(pxWhole.Y - pxFromPos.Y, pxFromSize.Y), 0);
            if (pxFromSize.X == 0 || pxFromSize.Y == 0)
            {
                return null;
            }
            System.Windows.Int32Rect rcFrom = new System.Windows.Int32Rect(pxFromPos.X, pxFromPos.Y, pxFromSize.X, pxFromSize.Y);

            RenderTargetBitmap rtb = new RenderTargetBitmap(pxWhole.X, pxWhole.Y, s_dpiX, s_dpiY, PixelFormats.Default);
            rtb.Render(AdornedElement);
            return new CroppedBitmap(rtb, rcFrom);
        }
        #endregion

        #region Helper functions
        private Thickness AdornerMargin()
        {
            Thickness thick = new Thickness(0);
            if (AdornedElement is FrameworkElement)
            {
                thick = ((FrameworkElement)AdornedElement).Margin;
            }
            return thick;
        }

        private void BuildCorner(ref CropThumb crt, Cursor crs)
        {
            if (crt != null) return;

            crt = new CropThumb(_cpxThumbWidth);

            // Set some arbitrary visual characteristics.
            crt.Cursor = crs;

            _cnvThumbs.Children.Add(crt);
        }

        private Point UnitsToPx(double x, double y)
        {
            return new Point((int)(x * s_dpiX / 96), (int)(y * s_dpiY / 96));
        }
        #endregion

        #region Visual tree overrides
        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return _vc.Count; } }
        protected override Visual GetVisualChild(int index) { return _vc[index]; }
        #endregion

        #region Internal Classes
        class CropThumb : Thumb
        {
            #region Private variables
            int _cpx;
            #endregion

            #region Constructor
            internal CropThumb(int cpx)
                : base()
            {
                _cpx = cpx;
            }
            #endregion

            #region Overrides
            protected override Visual GetVisualChild(int index)
            {
                return null;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawRoundedRectangle(Brushes.White, new Pen(Brushes.Black, 1), new Rect(new Size(_cpx, _cpx)), 1, 1);
            }
            #endregion

            #region Positioning
            internal void SetPos(double x, double y)
            {
                Canvas.SetTop(this, y - _cpx / 2);
                Canvas.SetLeft(this, x - _cpx / 2);
            }
            #endregion
        }
        #endregion
    }
}
