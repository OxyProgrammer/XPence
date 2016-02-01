/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.Utility;

namespace XPence.Infrastructure.MessagingService
{
    /// <summary>
    /// Default implementation of the IMessagingService interface.
    /// </summary>
    internal class MessagingService : IMessagingService
    {
        #region IMessagingService Members

        /// <summary>
        /// Shows a pogress message to the user.
        /// </summary>
        /// <param name="header">The header text for the dialog.</param>
        /// <param name="message">The message to be displayed in the dialog.</param>
        void IMessagingService.ShowProgressMessage(string header, string message)
        {
            _isMessageDialogVisible = true;
            _controller = _metroWindow.ShowProgressAsync(header, message);
        }

        /// <summary>
        /// Closes a progres message dialog that is visible.
        /// </summary>
        async void IMessagingService.CloseProgressMessage()
        {
            if (_isMessageDialogVisible)
            {
                _isMessageDialogVisible = false;
                if (null != _controller)
                {
                    var controller = await _controller;
                    await controller.CloseAsync();
                }
            }
        }

        /// <summary>
        /// A quick method to show a modal dialog to the user.
        /// The header text would be default.
        /// </summary>
        /// <param name="message">The message to be shown in the dialog.</param>
        void IMessagingService.ShowMessage(string message)
        {
            ShowMessage(message, DefaultHeaders.GetDefaultHeader(DialogType.Message), DialogType.Message);
        }

        /// <summary>
        /// A quick method to show a message to the user along with the header text.
        /// </summary>
        /// <param name="message">The message to be shown in the dialog.</param>
        /// <param name="header">The header of the dialog.</param>
        void IMessagingService.ShowMessage(string message, string header)
        {
            ShowMessage(message, header, DialogType.Message);
        }

        /// <summary>
        /// A method to show to the user a dialog with a  message. The method returns a dialog result.
        /// The header text would be default.
        /// </summary>
        /// <param name="message">The message to be shown in the dialog.</param>
        /// <param name="dialogueType">The header of the dialog.</param>
        /// <returns>The dialog result as selected by the user.</returns>
        DialogResponse IMessagingService.ShowMessage(string message, DialogType dialogueType)
        {
            return ShowMessage(message, DefaultHeaders.GetDefaultHeader(dialogueType), dialogueType);
        }

        /// <summary>
        /// A method to show to the user a dialog with a  message and header.
        /// The method returns a dialog result.
        /// </summary>
        /// <param name="message">The message to be shown in the dialog.</param>
        /// <param name="header">The header of the dialog.</param>
        /// <param name="dialogueType">The dialog type.</param>
        /// <returns>The dialog result as selected by the user.</returns>
        DialogResponse IMessagingService.ShowMessage(string message, string header, DialogType dialogueType)
        {
            return ShowMessage(message, header, dialogueType);
        }

        /// <summary>
        /// Shows a view identified by the <see cref="viewKey"/> and binds the view to the <see cref="viewModel"/>.
        /// The view is shown as a non blocking modal dialog.
        /// </summary>
        /// <param name="viewKey">shows the view identified by the key.</param>
        /// <param name="viewModel">The modal view is bound to this view model.</param>
        void IMessagingService.ShowCustomMessageDialog(string viewKey, ModalDialogViewModelBase viewModel)
        {
            var modalWindow = GetCustomWindow(viewKey, viewModel);
            modalWindow.ShowDialog();
        }

        /// <summary>
        /// Shows a view identified by the <see cref="viewKey"/> and binds the view to the <see cref="viewModel"/>.
        /// The view is shown as a blocking modal dialog.
        /// </summary>
        /// <param name="viewKey">shows the view identified by the key.</param>
        /// <param name="viewModel">The modal view is bound to this view model.</param>
        void IMessagingService.ShowCustomMessage(string viewKey, ModalDialogViewModelBase viewModel)
        {
            var modalWindow = GetCustomWindow(viewKey, viewModel);
            modalWindow.Show();
        }

        /// <summary>
        /// Registers an instance of <see cref="FlyoutViewModelBase"/> that can be used in the application.
        /// </summary>
        /// <param name="flyoutViewModelBase"></param>
        void IMessagingService.RegisterFlyout(FlyoutViewModelBase flyoutViewModelBase)
        {
            RegisterFlyout(flyoutViewModelBase);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the modal window that has to be shown to the user.
        /// </summary>
        /// <param name="viewKey"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private Window GetCustomWindow(string viewKey, ModalDialogViewModelBase viewModel)
        {
            if (string.IsNullOrEmpty(viewKey))
                throw new ArgumentException("viewKey");
            if (null == viewModel)
                throw new ArgumentException("viewModel");
            var viewRegistry = ModalViewRegistry.Instance;
            if (!viewRegistry.ContainsKey(viewKey))
                throw new ArgumentException(string.Format("the key is not present in the registry: {0}", viewKey));

            var userControl = viewRegistry.GetViewByKey(viewKey); // Get the UserControl.
            var modalWindow = new ModalCustomMessageDialog
            {
                // Set the content of the window as the user control
                DataContext = viewModel,
                // Set the data context of the window as the ViewModel
                Owner = Util.AppMainWindow,
                // Set the owner of the modal window to the app window.
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
                //Set content
                ActualContent = userControl,
                //Adjust height and width
                SizeToContent = SizeToContent.WidthAndHeight,
            };

            modalWindow.Closed += CleanModalWindow;
            return modalWindow;
        }

        /// <summary>
        /// The event handler that cleans up the references of the modal window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CleanModalWindow(object sender, EventArgs e)
        {
            var modalWindow = sender as ModalCustomMessageDialog;
            if (null != modalWindow)
            {
                modalWindow.Content = null;
                modalWindow.Owner = null;
                modalWindow.Closed -= CleanModalWindow;
            }
        }

        /// <summary>
        /// Show a dialog message to the user and returns the response.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="header"></param>
        /// <param name="dialogueType"></param>
        /// <returns></returns>
        private DialogResponse ShowMessage(string message, string header, DialogType dialogueType)
        {
            var result = new ModalMessageDialog().ShowMessage(message, header.ToUpper(),
                                                                         dialogueType, _metroWindow);
            return result;
        }

        /// <summary>
        /// All this method does is push the supplied instance to the MainViewModel's list of flyouts.
        /// </summary>
        /// <param name="flyoutViewModelBase"></param>
        private void RegisterFlyout(FlyoutViewModelBase flyoutViewModelBase)
        {
            if (flyoutViewModelBase == null) 
                throw new ArgumentNullException("flyoutViewModelBase");
            var flyoutContainer = Util.AppMainWindow.DataContext as IFlyoutContainer;
            if(flyoutContainer != null && flyoutContainer.Flyouts!=null)
            {
                flyoutContainer.Flyouts.Add(flyoutViewModelBase);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instances <see cref="MessagingService"/>.
        /// </summary>
        public MessagingService()
        {
            _metroWindow = Application.Current.MainWindow as MetroWindow;
            if (null == _metroWindow)
                throw new Exception("Failed to initialize Messaging Service");
            //All dialogs will be accented.If you want it themed, change the below line.
            _metroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
        }

        #endregion

        #region Member Variables

        private readonly MetroWindow _metroWindow;
        private Task<ProgressDialogController> _controller;
        private bool _isMessageDialogVisible;//This is required to get rid of any error that might be thrown doe to mahapps metro progress dialog implementation..

        #endregion
    }

    /// <summary>
    /// The class carries the default header information.
    /// </summary>
    internal static class DefaultHeaders
    {
        #region Static Propeties
        /// <summary>
        /// Gets the default header for information dialog.
        /// </summary>
        public static string DefaultInformationHeader { get; private set; }

        /// <summary>
        /// Gets the default header for error dialog.
        /// </summary>
        public static string DefaultErrorHeader { get; private set; }


        /// <summary>
        /// Gets the default header for question dialog.
        /// </summary>
        public static string DefaultQuestionHeader { get; private set; }
        #endregion

        #region Static Constructor
        /// <summary>
        /// Initializes the sttic values.
        /// </summary>
        static DefaultHeaders()
        {
            DefaultInformationHeader = "Information";
            DefaultErrorHeader = "Error";
            DefaultQuestionHeader = "Confirm";
        }
        #endregion

        #region Static Method
        /// <summary>
        /// A utility method to quickly return the default header text on supplying DialougeType.
        /// </summary>
        /// <param name="dialogueType"></param>
        /// <returns></returns>
        public static string GetDefaultHeader(DialogType dialogueType)
        {
            string retValue = null;
            switch (dialogueType)
            {
                case DialogType.Message:
                    retValue = DefaultInformationHeader;
                    break;
                case DialogType.Error:
                    retValue = DefaultErrorHeader;
                    break;
                case DialogType.Question:
                    retValue = DefaultQuestionHeader;
                    break;
            }
            return retValue;
        }
        #endregion

    }
}
