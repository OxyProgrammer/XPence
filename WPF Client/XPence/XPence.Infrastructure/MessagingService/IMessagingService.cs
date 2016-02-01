/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using XPence.Infrastructure.BaseClasses;

namespace XPence.Infrastructure.MessagingService
{
    /// <summary>
    /// The messaging service interface.
    /// </summary>
    public interface IMessagingService
    {
        /// <summary>
        /// A quick method to show a modal dialog to the user.
        /// The header text would be default.
        /// </summary>
        /// <param name="message">The message to be shown in the dialog.</param>
        void ShowMessage(string message);

        /// <summary>
        /// A quick method to show a message to the user along with the header text.
        /// </summary>
        /// <param name="message">The message to be shown in the dialog.</param>
        /// <param name="header">The header of the dialog.</param>
        void ShowMessage(string message, string header);

        /// <summary>
        /// A method to show to the user a dialog with a  message. The method returns a dialog result.
        /// The header text would be default.
        /// </summary>
        /// <param name="message">The message to be shown in the dialog.</param>
        /// <param name="dialogueType">The header of the dialog.</param>
        /// <returns>The dialog result as selected by the user.</returns>
        DialogResponse ShowMessage(string message, DialogType dialogueType);

        /// <summary>
        /// A method to show to the user a dialog with a  message and header.
        /// The method returns a dialog result.
        /// </summary>
        /// <param name="message">The message to be shown in the dialog.</param>
        /// <param name="header">The header of the dialog.</param>
        /// <param name="dialogueType">The dialog type.</param>
        /// <returns>The dialog result as selected by the user.</returns>
        DialogResponse ShowMessage(string message, string header, DialogType dialogueType);

        /// <summary>
        ///  A method to display wait dialog to the user.
        /// </summary>
        /// <param name="showBusy"></param>
        /// <param name="message"></param>
        /// <param name="header"></param>
        void ShowProgressMessage(string header, string message);

        /// <summary>
        /// Closes a progres message dialog that is visible.
        /// </summary>
        void CloseProgressMessage();

        /// <summary>
        /// Shows a view identified by the <see cref="viewKey"/> and binds the view to the <see cref="viewModel"/>.
        /// The view is shown as a blocking modal dialog.
        /// </summary>
        /// <param name="viewKey">shows the view identified by the key.</param>
        /// <param name="viewModel">The modal view is bound to this view model.</param>
        void ShowCustomMessageDialog(string viewKey, ModalDialogViewModelBase viewModel);

        /// <summary>
        /// Shows a view identified by the <see cref="viewKey"/> and binds the view to the <see cref="viewModel"/>.
        /// The view is shown as a non blocking modal dialog.
        /// </summary>
        /// <param name="viewKey">shows the view identified by the key.</param>
        /// <param name="viewModel">The modal view is bound to this view model.</param>
        void ShowCustomMessage(string viewKey, ModalDialogViewModelBase viewModel);

        /// <summary>
        /// Registers an instance of <see cref="FlyoutViewModelBase"/> that can be used in the application.
        /// </summary>
        /// <param name="flyoutViewModelBase"></param>
        void RegisterFlyout(FlyoutViewModelBase flyoutViewModelBase);
    }
}
