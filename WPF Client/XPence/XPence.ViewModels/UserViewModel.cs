/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Linq;
using System.Windows.Input;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.CoreClasses;
using XPence.Infrastructure.MessagingService;
using XPence.Models.DataModels;
using XPence.Shared;

namespace XPence.ViewModels
{
    /// <summary>
    /// A UI friendly wrapper around the <see cref="User"/> odel.
    /// </summary>
    public class UserViewModel : ViewModelBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string Username
        {
            get { return Entity.Username; }
            set
            {
                if (value == Entity.Username)
                    return;
                Entity.Username = value;
                OnPropertyChanged(GetPropertyName(() => Username));
            }
        }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password
        {
            get { return Entity.Password; }
            set
            {
                if (value == Entity.Password)
                    return;
                Entity.Password = value;
                OnPropertyChanged(GetPropertyName(() => Password));
            }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name
        {
            get { return Entity.Name; }
            set
            {
                if (value == Entity.Name)
                    return;
                Entity.Name = value;
                OnPropertyChanged(GetPropertyName(() => Name));
            }
        }

        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        public UserRole Role
        {
            get { return Entity.Role; }
            set
            {
                if (value == Entity.Role)
                    return;
                Entity.Role = value;
                OnPropertyChanged(GetPropertyName(() => Role));
            }
        }

        /// <summary>
        /// Gets or sets the theme selcted by the user.
        /// </summary>
        public string SelectedTheme
        {
            get { return Entity.SelectedTheme; }
            set
            {
                if (value == Entity.SelectedTheme)
                    return;
                Entity.SelectedTheme = value;
                OnPropertyChanged(GetPropertyName(() => SelectedTheme));
            }
        }

        /// <summary>
        /// Gets or sets the accent selected by the user.
        /// </summary>
        public string SelectedAccent
        {
            get { return Entity.SelectedAccent; }
            set
            {
                if (value == Entity.SelectedAccent)
                    return;
                Entity.SelectedAccent = value;
                OnPropertyChanged(GetPropertyName(() => SelectedTheme));
            }
        }

        /// <summary>
        /// Gets or sets the byte array carrying the Image data.
        /// </summary>
        public byte[] Picture
        {
            get { return Entity.Picture; }
            set
            {
                Entity.Picture = value;
                OnPropertyChanged(GetPropertyName(() => Picture));
            }
        }

        /// <summary>
        /// Gets the wrapped up model instance.
        /// </summary>
        public User Entity { get; private set; }

        /// <summary>
        /// Returns if this instance of <see cref="TransactionViewModel"/> is valid for saving.
        /// </summary>
        public bool IsValid
        {
            get { return _propertyNames.All(p => GetErrorForProperty(p) == null); }
        }

        /// <summary>
        /// Gets if the username of a user can be edited.
        /// </summary>
        public bool IsUsernameEditable
        {
            get { return Entity.UserId < 1; }
            //get { return _isUsernameEditable; }
            //set
            //{
            //    if (value == _isUsernameEditable)
            //        return;
            //    _isUsernameEditable = value;
            //    OnPropertyChanged(GetPropertyName(() => IsUsernameEditable));
            //}
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to change the image of the user.
        /// </summary>
        public ICommand ChangeImageCommand { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Refreshes the UserViewModel instance.
        /// </summary>
        public void Refresh()
        {
            OnPropertyChanged(GetPropertyName(() => IsUsernameEditable));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Intializes an instance of <see cref="UserViewModel"/>.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="messagingService"> </param>
        public UserViewModel(User user, IMessagingService messagingService)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (messagingService == null)
                throw new ArgumentNullException("messagingService");
            _messagingService = messagingService;
            Entity = user;
            //IsUsernameEditable=Entity.UserId <1;
            //Initialize commands
            ChangeImageCommand = new RelayCommand(ChangeImage);
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Gets the error string for a property value against a property's name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string GetErrorForProperty(string propertyName)
        {
            CommandManager.InvalidateRequerySuggested();
            return Entity[propertyName];
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Changes the image of the user.
        /// </summary>
        private void ChangeImage()
        {
            var imagePickerViewModel = new ImagePickerViewModel() { TitleText = UIText.IMAGE_PICKER_WINDOW_TITLE };
            _messagingService.ShowCustomMessageDialog(Constants.PICTURE_PICKER_VIEW, imagePickerViewModel);
            if (imagePickerViewModel.IsOk)
            {
                var imageData = imagePickerViewModel.Image;
                if (null != imageData)
                    Picture = imageData;
            }
        }

        #endregion

        #region Member Variables

        private static readonly string[] _propertyNames = { "Name", "Username" };
        private readonly IMessagingService _messagingService;

        #endregion
    }
}
