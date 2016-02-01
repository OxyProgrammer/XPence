/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using XPence.Infrastructure.MessagingService;

namespace XPence.ViewModels
{
    /// <summary>
    /// A view model class to cater to the image picker secondary view.
    /// </summary>
    public class ImagePickerViewModel:ModalDialogViewModelBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the byte array carrying the Image data.
        /// </summary>
        public byte[] Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(GetPropertyName(() => Image));
            }
        }

        #endregion

        #region Member Variables

        private byte[] _image;

        #endregion
    }
}
