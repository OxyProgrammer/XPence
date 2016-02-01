/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Input;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.CoreClasses;
using XPence.Infrastructure.MessagingService;
using XPence.Models.Interfaces;
using XPence.Shared;
using XPence.Infrastructure.Utility;

namespace XPence.ViewModels
{
    /// <summary>
    /// This view model class caters to the settings view.
    /// </summary>
    public class SettingsViewModel : FlyoutViewModelBase
    {
        #region Public properties

        /// <summary>
        /// Gets the list of the accent color names.
        /// </summary>
        public IList<string> AccentColorlist { get; private set; }

        /// <summary>
        /// Gets the name of the base themes.
        /// </summary>
        public IList<string> ThemeColorlist { get; private set; }

        /// <summary>
        /// Gets or sets the selected accent color.
        /// </summary>
        public string SelectedAccent
        {
            get { return _selectedAccent; }
            set
            {
                if (value == _selectedAccent)
                    return;
                _selectedAccent = value;
                OnPropertyChanged(GetPropertyName(() => SelectedAccent));
                AccentChangeRequested();
            }
        }

        /// <summary>
        /// Gets or sets the selected theme.
        /// </summary>
        public string SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                if (value == _selectedTheme)
                    return;
                _selectedTheme = value;
                OnPropertyChanged(GetPropertyName(() => SelectedTheme));
                ThemeChangeRequested();
            }
        }

        /// <summary>
        /// Gets or sets the old passwod by the user.
        /// </summary>
        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                if (value == _oldPassword)
                    return;
                _oldPassword = value;
                OnPropertyChanged(GetPropertyName(() => OldPassword));
            }
        }

        /// <summary>
        /// Gets or sets the new password by the user.
        /// </summary>
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                if (value == _newPassword)
                    return;
                _newPassword = value;
                OnPropertyChanged(GetPropertyName(() => NewPassword));
                OnPropertyChanged(GetPropertyName(() => ReenterNewPassword));
            }
        }

        /// <summary>
        /// Gets or sets the password that is re entered by the user.
        /// </summary>
        public string ReenterNewPassword
        {
            get { return _reenterNewPassword; }
            set
            {
                if (value == _reenterNewPassword)
                    return;
                _reenterNewPassword = value;
                OnPropertyChanged(GetPropertyName(() => ReenterNewPassword));
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to change the password.
        /// </summary>
        public ICommand ChangePasswordCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="SettingsViewModel"/>.
        /// </summary>
        /// <param name="messagingService">An implementation of <see cref="IMessagingService"/>.</param>
        /// <param name="userRepository"> </param>
        public SettingsViewModel(IMessagingService messagingService, IUserRepository userRepository)
        {
            if (null == messagingService)
                throw new ArgumentNullException("messagingService");
            if (userRepository == null)
                throw new ArgumentNullException("userRepository");
            _messagingService = messagingService;
            _userRepository = userRepository;
            AccentColorlist = new List<string>(AppearanceManager.GetAccentNames());
            ThemeColorlist = new List<string>(AppearanceManager.GetThemeNames());
            _selectedAccent = AppearanceManager.GetApplicationAccent();
            _selectedTheme = AppearanceManager.GetApplicationTheme();
            //Initialize commands
            ChangePasswordCommand = new RelayCommand(ChangePassword, CanChangePassword);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the default theme for this instance of <see cref="SettingsViewModel"/>
        /// </summary>
        /// <param name="themeName"></param>
        /// <param name="accentName"></param>
        public void SetDefaultSettings(string themeName, string accentName)
        {
            if (themeName == null)
                throw new ArgumentNullException("themeName");
            if (accentName == null)
                throw new ArgumentNullException("accentName");
            _selectedTheme = themeName;
            OnPropertyChanged(GetPropertyName(() => SelectedTheme));
            _selectedAccent = accentName;
            OnPropertyChanged(GetPropertyName(() => SelectedAccent));
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
            string error = null;
            switch (propertyName)
            {
                case "ReenterNewPassword":
                    if (string.IsNullOrEmpty(ReenterNewPassword))
                        error = ErrorMessages.ERR_NEW_PASSORD_EMPTY;
                    if (string.Compare(ReenterNewPassword, NewPassword, false) != 0)
                        error = ErrorMessages.ERR_PASSWORDS_MISMATCH;
                    break;
            }
            CommandManager.InvalidateRequerySuggested();
            return error;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Sets the requested theme to the application's appearance.
        /// </summary>
        private void ThemeChangeRequested()
        {
            AppearanceManager.ChangeTheme(SelectedTheme);
            PromptUserToSaveAppearance();
        }

        /// <summary>
        /// Sets the requested accent to the application's appearance.
        /// </summary>
        private void AccentChangeRequested()
        {
            AppearanceManager.ChangeAccent(SelectedAccent);
            PromptUserToSaveAppearance();
        }

        /// <summary>
        /// Prompt the user if he wishes to persist the appearnce for future too.
        /// </summary>
        private void PromptUserToSaveAppearance()
        {
            if (_messagingService.ShowMessage(UIText.APPEAR_CHANGE_PROMPT, DialogType.Question) == DialogResponse.Yes)
            {
                LogUtil.LogInfo("SettingsViewModel", "PromptUserToSaveAppearance", "User changed apperance.");
                try
                {
                    AppData.LoggedInUser.SelectedAccent = SelectedAccent;
                    AppData.LoggedInUser.SelectedTheme = SelectedTheme;
                    _userRepository.SaveUser(AppData.LoggedInUser);
                }
                catch (Exception ex)
                {
                    LogUtil.LogError("SettingsViewModel", "PromptUserToSaveAppearance",ex);
                    _messagingService.ShowMessage(ErrorMessages.ERR_APP_ERROR, DialogType.Error);
                }
            }
        }

        /// <summary>
        /// Changes the password of the logged in user.
        /// </summary>
        private void ChangePassword()
        {
            if(AppData.LoggedInUser!=null)
            {
                try
                {
                    LogUtil.LogInfo("SettingsViewModel", "PromptUserToSaveAppearance", "User attempted password change.");
                    if (string.Compare(AppData.LoggedInUser.Password, OldPassword, false) != 0)
                    {
                        _messagingService.ShowMessage(ErrorMessages.ERR_PASSWOR_NO_MATCH_MSG, DialogType.Error);
                        return;
                    }
                    AppData.LoggedInUser.Password = NewPassword;
                    _userRepository.SaveUser(AppData.LoggedInUser);
                    _messagingService.ShowMessage(InfoMessages.INF_PASSWORD_CHANGED_MSG);
                    //Clear the passwords
                    OldPassword = NewPassword = ReenterNewPassword = null;
                }
                catch (Exception ex)
                {
                    LogUtil.LogError("SettingsViewModel", "ChangePassword", ex);
                    _messagingService.ShowMessage(ErrorMessages.ERR_APP_ERROR, DialogType.Error);
                }
            }
            else
            {
                _messagingService.ShowMessage(ErrorMessages.ERR_APP_ERROR, DialogType.Error);
            }
        }

        /// <summary>
        /// Evaluates if the password change command can be fireed.
        /// </summary>
        /// <returns></returns>
        private bool CanChangePassword()
        {
            return GetErrorForProperty("ReenterNewPassword") == null;
        }

        #endregion

        #region Member Variables

        private string _selectedAccent;
        private string _selectedTheme;
        private string _oldPassword;
        private string _newPassword;
        private string _reenterNewPassword;
        private readonly IMessagingService _messagingService;
        private readonly IUserRepository _userRepository;

        #endregion
    }
}
