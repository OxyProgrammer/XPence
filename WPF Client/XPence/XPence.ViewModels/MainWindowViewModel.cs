/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using XPence.Shared;
using XPence.Models;
using XPence.Models.EventArgsAndException;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.CoreClasses;
using XPence.Infrastructure.MessagingService;
using XPence.Infrastructure.Utility;

namespace XPence.ViewModels
{
    /// <summary>
    /// The ViewModel class that caters to the main window of the application.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase, IFlyoutContainer
    {
        #region Public Members

        /// <summary>
        /// Gets or sets an instance of <see cref="WorkspaceViewModelBase"/> That is shown in the MainWindow.
        /// </summary>
        public WorkspaceViewModelBase SelectedView
        {
            get { return _selectedView; }
            set
            {
                if (_selectedView == value)
                    return;
                _selectedView = value;
                OnPropertyChanged(GetPropertyName(() => SelectedView));
            }
        }

        /// <summary>
        /// Gets an instance of <see cref="SettingsViewModel"/> that caters to the SettingsView.
        /// </summary>
        public SettingsViewModel SettingsView { get; private set; }

        /// <summary>
        /// Gets or sets if the user is logged in.
        /// </summary>
        public bool IsUserLoggedIn
        {
            get { return _isUserLoggedIn; }
            set
            {
                if (_isUserLoggedIn == value)
                    return;
                _isUserLoggedIn = value;
                OnPropertyChanged(GetPropertyName(() => IsUserLoggedIn));
            }
        }

        /// <summary>
        /// Gets or sets the list of <see cref="FlyoutViewModelBase"/>
        /// </summary>
        public ObservableCollection<FlyoutViewModelBase> Flyouts
        {
            get { return _flyouts; }
            set
            {
                if (value == _flyouts)
                    return;
                _flyouts = value;
                OnPropertyChanged(GetPropertyName(() => Flyouts));
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to toggle the visibility of the settings view.
        /// </summary>
        public ICommand ToggleSettingsVisibilityCommand { get; private set; }

        /// <summary>
        /// Gets the command to show the help.
        /// </summary>
        public ICommand ShowHelpCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="MainWindowViewModel"/>.
        /// </summary>
        /// <param name="messagingService"></param>
        public MainWindowViewModel(IMessagingService messagingService)
        {
            if (null == messagingService)
                throw new ArgumentNullException("messagingService");
            _messagingService = messagingService;
            _loginView = new LoginViewModel(RepositoryFactory.GetUserRepository(), _messagingService);
            SelectedView = _loginView;
            SettingsView = new SettingsViewModel(_messagingService, RepositoryFactory.GetUserRepository())
                               {
                                   Position = VisibilityPosition.Right,
                                   Theme = FlyoutTheme.AccentedTheme,
                                   Header = UIText.SETTINGS_VIEW_HEADER
                               };
            IsUserLoggedIn = false;
            //Initialize commands
            ToggleSettingsVisibilityCommand = new RelayCommand(() => SettingsView.IsOpen = !SettingsView.IsOpen, () => IsUserLoggedIn);
            ShowHelpCommand = new RelayCommand(ShowHelp);
            //SessionProvider.RebuildSchema();
        }

        #endregion

        #region Overriden methods

        /// <summary>
        /// A place holder for self initialization.
        /// </summary>
        protected override void OnInitialize()
        {
            Flyouts = new ObservableCollection<FlyoutViewModelBase>();
            _loginView.UserLoggedIn += LoginViewUserLoggedIn;
            _messagingService.RegisterFlyout(SettingsView);
        }

        /// <summary>
        /// Clean-up!
        /// </summary>
        protected override void OnDispose()
        {
            //Clear the flyouts
            Flyouts.Clear();
            //Dispose the login view
            if (null != _loginView)
            {
                _loginView.UserLoggedIn -= LoginViewUserLoggedIn;
                _loginView.Dispose();
            }
            //Dispose the application view.
            if (null != SelectedView)
            {
                SelectedView.Dispose();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the event of user successfully logged in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginViewUserLoggedIn(object sender, UserLoggedInEventArgs e)
        {
            AppData.LoggedInUser = e.LoggedInUser;
            IsUserLoggedIn = true;
            SetUserPreference();
            var applicationViewModel = new ApplicationViewModel(new UserViewModel(e.LoggedInUser, _messagingService),RepositoryFactory.GetTransactionRepository(),
                                                                RepositoryFactory.GetUserRepository(), _messagingService);
            SelectedView = applicationViewModel;
            //SelectedView.Initialize();
            //applicationViewModel.SetUser();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the appearance of the application as per user prefernce.
        /// </summary>
        private void SetUserPreference()
        {
            string accent = AppData.LoggedInUser.SelectedAccent ?? AppearanceManager.GetApplicationAccent();
            string theme = AppData.LoggedInUser.SelectedTheme ?? AppearanceManager.GetApplicationTheme();
            AppearanceManager.ChangeAccent(accent);
            AppearanceManager.ChangeTheme(theme);
            //Set the defaults in settings view too!
            SettingsView.SetDefaultSettings(AppearanceManager.GetApplicationTheme(),AppearanceManager.GetApplicationAccent());
        }

        private void ShowHelp()
        {
            try
            {
                _messagingService.ShowCustomMessageDialog(Constants.HELP_VIEW, new HelpViewModel() { TitleText = UIText.ABOUT_WINDOW_HEADER });
            }
            catch (Exception ex)
            {
                LogUtil.LogError("LoginViewModel", "TryLoginIn", ex);
                _messagingService.ShowMessage(ErrorMessages.ERR_APP_ERROR, DialogType.Error);
            }
        }

        #endregion

        #region Members variables

        private readonly LoginViewModel _loginView;
        private WorkspaceViewModelBase _selectedView;
        private bool _isUserLoggedIn;
        private readonly IMessagingService _messagingService;
        private ObservableCollection<FlyoutViewModelBase> _flyouts;

        #endregion
    }
}
