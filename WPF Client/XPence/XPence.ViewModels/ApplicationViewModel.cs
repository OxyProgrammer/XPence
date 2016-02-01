/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using XPence.Models.Interfaces;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.CoreClasses;
using XPence.Infrastructure.MessagingService;
using XPence.Infrastructure.Navigation;
using XPence.Shared;

namespace XPence.ViewModels
{
    /// <summary>
    /// A view model class that renders the Application after the user has logged in.
    /// </summary>
    public class ApplicationViewModel : WorkspaceViewModelBase
    {
        #region Public Properties

        /// <summary>
        /// Gets the instance of Navigator that is responsible for all navigation purpose.
        /// </summary>
        public INavigator Navigator { get; private set; }

        /// <summary>
        /// Gets or sets the selected workspace.
        /// </summary>
        public WorkspaceViewModelBase SelectedWorkspace
        {
            get { return _selectedWorkspace; }
            set
            {
                if (_selectedWorkspace == value)
                    return;
                _selectedWorkspace = value;
                OnPropertyChanged(GetPropertyName(() => SelectedWorkspace));
            }
        }

        /// <summary>
        /// Gets the logged in user.
        /// </summary>
        public UserViewModel LoggedInUser
        {
            get { return _loggedInUser; }
            private set
            {
                if (value == _loggedInUser)
                    return;
                _loggedInUser = value;
                OnPropertyChanged(GetPropertyName(() => LoggedInUser));
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to Navigat back.
        /// </summary>
        public ICommand NavigateBackCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="ApplicationViewModel"/>.
        /// </summary>
        /// <param name="user">The view model that wraps the logged in user model. </param>
        /// <param name="transactionRepository">An instance of <see cref="ITransactionRepository"/> implementation.</param>
        /// <param name="userRepository"> >An instance of <see cref="IUserRepository"/> implementation.</param>
        /// <param name="messagingService">An implementation of <see cref="IMessagingService"/> </param>
        public ApplicationViewModel(UserViewModel user,ITransactionRepository transactionRepository, IUserRepository userRepository, IMessagingService messagingService)
            : base("ApplicationViewModel")
        {
            LoggedInUser = user;
            bool isUserAdmin = user.Role == UserRole.Admin;
            //Configure the navigator
            Navigator = NavigatorFactory.GetNavigator();
            var viewList = new List<WorkspaceViewModelBase>()
                               {
                                   new AllTransactionViewModel(isUserAdmin,Constants.ALLEXPENSES_VIEW_REGERED_NAME,userRepository,transactionRepository,messagingService),
                                   new ManageViewModel(Constants.MANAGE_VIEW_REGERED_NAME,userRepository,messagingService,isUserAdmin)
                               };
            viewList.ForEach(wvm => Navigator.AddView(wvm));
            Navigator.AddHomeView(new HomeViewModel(viewList, Constants.HOME_VIEW_REGERED_NAME, messagingService));
            Navigator.PropertyChanged += NavigatorPropertyChanged;
            Navigator.NavigateToHome();
            //Initialie the commands
            NavigateBackCommand = new RelayCommand(Navigator.NavigateBack);
        }

        #endregion

        #region Private event handler

        /// <summary>
        /// Handles the the event of the Navigator instance property changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetPropertyName(() => Navigator.CurrentView))
            {
                SelectedWorkspace = Navigator.CurrentView;
            }
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Clean Up!
        /// </summary>
        protected override void OnDispose()
        {
            Navigator.PropertyChanged -= NavigatorPropertyChanged;
        }

        #endregion

        #region Member Variables

        private WorkspaceViewModelBase _selectedWorkspace;
        private UserViewModel _loggedInUser;

        #endregion
    }
}
