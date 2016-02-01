using System;
using System.Windows;

namespace XPence.Infrastructure.MessagingService
{
    /// <summary>
    /// Interaction logic for ModalCustomMessageDialog.xaml
    /// </summary>
    public partial class ModalCustomMessageDialog
    {
        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static ModalCustomMessageDialog()
        {
        }

        /// <summary>
        /// Initializes and ionstance of <see cref="ModalCustomMessageDialog"/>.
        /// </summary>
        public ModalCustomMessageDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Static Event Handlers

        /// <summary>
        /// The handler to handle the content property changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnActualContentPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var modalWindow = source as ModalCustomMessageDialog;
            if (null != modalWindow)
            {
                if (null != modalWindow.ActualContentHolder)
                    modalWindow.ActualContentHolder.Content = e.NewValue;
            }
        }

        #endregion

        /// <summary>
        /// A duplicate content property built to help a developer with his laziness.
        /// </summary>
        public object ActualContent
        {
            get { return GetValue(ActualContentProperty); }
            set { SetValue(ActualContentProperty, value); }
        }
        public static readonly DependencyProperty ActualContentProperty = DependencyProperty.Register("ActualContent", typeof(object), typeof(ModalCustomMessageDialog), new PropertyMetadata(null, OnActualContentPropertyChanged));

        #region Base class overrides

        /// <summary>
        /// On property changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == DataContextProperty)
            {
                var oldViewModel = e.OldValue as ModalDialogViewModelBase;
                if (null != oldViewModel)
                    oldViewModel.DialogResultSelected -= OnViewNotified;
                var newViewModel = e.NewValue as ModalDialogViewModelBase;
                if (null != newViewModel)
                    newViewModel.DialogResultSelected += OnViewNotified;
            }
            else
            {
                base.OnPropertyChanged(e);
            }
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Closes the Window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnViewNotified(object sender, EventArgs e)
        {
            var viewModel = DataContext as ModalDialogViewModelBase;
            if (null != viewModel)
                viewModel.DialogResultSelected -= OnViewNotified;
            Close(); //Close the window
        }

        #endregion
    }
}

