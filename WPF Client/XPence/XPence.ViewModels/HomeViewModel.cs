/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.CoreClasses;
using XPence.Infrastructure.MessagingService;
using XPence.Infrastructure.Navigation;
using XPence.Shared;

namespace XPence.ViewModels
{
    /// <summary>
    /// A view model catering to the home view
    /// </summary>
    public sealed class HomeViewModel : WorkspaceViewModelBase
    {
        #region  Public Properties

        /// <summary>
        /// Gets or sets the collection of the view
        /// </summary>
        public ObservableCollection<WorkspaceViewModelBase> AllViews
        {
            get { return _allViews; }
            set
            {
                if (_allViews == value)
                    return;
                _allViews = value;
                OnPropertyChanged(GetPropertyName(() => AllViews));
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to go to a view.
        /// </summary>
        public ICommand GoToCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="HomeViewModel"/>.
        /// </summary>
        /// <param name="viewList">The enumerable of all other view than the home view.</param>
        /// <param name="messagingService">An implementation of <see cref="IMessagingService"/>.</param>
        public HomeViewModel(IEnumerable<WorkspaceViewModelBase> viewList, string registeredName, IMessagingService messagingService)
            : base(registeredName)
        {
            if (null == viewList)
                throw new ArgumentNullException("viewList");
            if (messagingService == null)
                throw new ArgumentNullException("messagingService");
            _messagingService = messagingService;
            DisplayName = UIText.HOME_VIEW_HEADER;
            CanGoBack = false;
            AllViews = new ObservableCollection<WorkspaceViewModelBase>(viewList);
            //Command Initialization
            GoToCommand = new RelayCommand<string>(GoToView);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Navigates to a view chosen by the user.
        /// </summary>
        /// <param name="viewRegisteredName"></param>
        private void GoToView(string viewRegisteredName)
        {
            var navigator = NavigatorFactory.GetNavigator();

            var wvm = AllViews.FirstOrDefault(vm => vm.RegisteredName == viewRegisteredName);
            if (wvm != null && !wvm.CanUserNavigate)
            {
                _messagingService.ShowMessage(InfoMessages.INF_NOT_AUTORIZED_MESSAGE);
                return;
            }
            navigator.NavigateToView(viewRegisteredName);
        }

        #endregion

        #region Member Variables

        private readonly IMessagingService _messagingService;
        private ObservableCollection<WorkspaceViewModelBase> _allViews;

        #endregion
    }
}
