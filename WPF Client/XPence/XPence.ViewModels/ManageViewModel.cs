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
using XPence.Infrastructure.Utility;
using XPence.Models.EventArgsAndException;
using XPence.Models.Interfaces;
using XPence.Shared;

namespace XPence.ViewModels
{
    /// <summary>
    /// A view model to cater to the manage view.
    /// </summary>
    public class ManageViewModel : WorkspaceViewModelBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the list of the users.
        /// </summary>
        public ExtendedObservableCollection<UserViewModel> Users
        {
            get { return _users; }
            set
            {
                if (_users == value)
                    return;
                _users = value;
                OnPropertyChanged(GetPropertyName(() => Users));
            }
        }

        /// <summary>
        /// Gets or sets the selected user.
        /// </summary>
        public UserViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser == value)
                    return;
                _selectedUser = value;
                OnPropertyChanged(GetPropertyName(() => SelectedUser));
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to refresh the users list.
        /// </summary>
        public ICommand RefreshUserListCommand { get; private set; }

        /// <summary>
        /// Gets the command to save a user.
        /// </summary>
        public ICommand SaveUsersCommand { get; private set; }

        /// <summary>
        /// Gets the command to add a new user.
        /// </summary>
        public ICommand AddNewUserCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="ManageViewModel"/>.
        /// </summary>
        /// <param name="registeredName"></param>
        /// <param name="userRepository"></param>
        /// <param name="messagingService"></param>
        /// <param name="canUserNavigate"></param>
        public ManageViewModel(string registeredName, IUserRepository userRepository, IMessagingService messagingService,bool canUserNavigate)
            : base(registeredName,canUserNavigate)
        {
            if (userRepository == null)
                throw new ArgumentNullException("userRepository");
            if (messagingService == null)
                throw new ArgumentNullException("messagingService");
            _userRepository = userRepository;
            _messagingService = messagingService;
            DisplayName = UIText.MANAGE_VIEW_HEADER;
            CanGoBack = true;
            Users = new ExtendedObservableCollection<UserViewModel>();
            //Intialize commands 
            RefreshUserListCommand = new RelayCommand(RefreshUserList);
            SaveUsersCommand = new RelayCommand(SaveUser, CanSaveUser);
            AddNewUserCommand = new RelayCommand(() => SelectedUser = new UserViewModel(_userRepository.GetNewUser(),_messagingService));
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// On Initialize
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            _userRepository.GetAllUsersCompleted += UserRepositoryGetAllUsersCompleted;
            _userRepository.SaveUserCompleted += UserRepositorySaveUserCompleted;
            RefreshUserList();
        }

        /// <summary>
        /// Clean up!
        /// </summary>
        protected override void OnDispose()
        {
            _userRepository.GetAllUsersCompleted -= UserRepositoryGetAllUsersCompleted;
            _userRepository.SaveUserCompleted -= UserRepositorySaveUserCompleted;
            base.OnDispose();
        }

        #endregion

        #region Private Event handlers.

        /// <summary>
        /// This handles when the <see cref="IUserRepository.GetAllUsersCompleted"/> is raise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserRepositorySaveUserCompleted(object sender, RepositoryTaskFinishedEventArgs e)
        {
            ThreadSafeInvoke(() =>
            {
                _messagingService.CloseProgressMessage();
                if (e.HasError)
                {
                    _messagingService.ShowMessage(UIText.ERROR_OCCURED_MSG);
                }
                if (!Users.Contains(SelectedUser))
                {
                    Users.Add(SelectedUser);
                    OnPropertyChanged(GetPropertyName(() => SelectedUser));
                    SelectedUser.Refresh();
                }
            });
        }

        /// <summary>
        /// This handles when the <see cref="IUserRepository.SaveUserCompleted"/> is raise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserRepositoryGetAllUsersCompleted(object sender, GetAllUsersTasskFinishedEventArg e)
        {
            ThreadSafeInvoke(() =>
            {
                _messagingService.CloseProgressMessage();
                if (e.HasError)
                {
                    _messagingService.ShowMessage(UIText.ERROR_OCCURED_MSG);
                    return;
                }
                if (e.UserList == null || !e.UserList.Any())
                {
                    _messagingService.ShowMessage(UIText.NO_DATA_EXISTS_MSG);
                    return;
                }
                var userList = e.UserList.Select(u => new UserViewModel(u,_messagingService));
                Users.AddRange(userList);
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the list of users.
        /// </summary>
        private void RefreshUserList()
        {
            SelectedUser = null;
            Users.Clear();
            _messagingService.ShowProgressMessage(UIText.WAIT_SCREEN_HEADER, UIText.GETTING_USERS_WAIT_MSG);
            _userRepository.GetAllUsersAsync();
        }

        /// <summary>
        /// Saves the user.
        /// </summary>
        private void SaveUser()
        {
            try
            {
                LogUtil.LogInfo("ManageViewModel", "SaveUser", "Checking for username");
                //Check if the username exists.
                if (SelectedUser.Entity.UserId<1 && _userRepository.CheckIfUserNameExists(SelectedUser.Username))
                {
                    LogUtil.LogInfo("ManageViewModel", "SaveUser", "Checking for username failed.");
                    _messagingService.ShowMessage(InfoMessages.INF_USERNAME_DUP_MSG);
                    return;
                }
                //Save the user.
                _messagingService.ShowProgressMessage(UIText.WAIT_SCREEN_HEADER, UIText.SAVING_USER_WAIT_MSG);
                _userRepository.SaveUserAsync(SelectedUser.Entity);
                LogUtil.LogInfo("ManageViewModel", "SaveUser", string.Format("Saved user successfully: {0}.", SelectedUser.Username));
            }
            catch (Exception ex)
            {
                LogUtil.LogError("ManageViewModel", "SaveUser", ex);
            }
        }

        /// <summary>
        /// Gets if the user can be saved.
        /// </summary>
        /// <returns></returns>
        private bool CanSaveUser()
        {
            if (null == SelectedUser)
                return false;
            return SelectedUser.IsValid;
        }

        #endregion

        #region Members Variables

        private ExtendedObservableCollection<UserViewModel> _users;
        private UserViewModel _selectedUser;
        private readonly IUserRepository _userRepository;
        private readonly IMessagingService _messagingService;

        #endregion
    }
}
