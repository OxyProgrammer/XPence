/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XPence.Infrastructure.BaseClasses;

namespace XPence.Infrastructure.Navigation
{
    /// <summary>
    /// The implementation of the contract <see cref="INavigator"/>.
    /// this class has no need on its ownself, hence explicit implementation.
    /// </summary>
    internal class Navigator : INavigator
    {
        #region INavigator Implementation

        /// <summary>
        /// 
        /// </summary>
        void INavigator.NavigateBack()
        {
            if (_currentView.CanGoBack)
            {
                NavigateToHome();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        WorkspaceViewModelBase INavigator.CurrentView
        {
            get { return _currentView; }
            set
            {
                if (value == _currentView)
                    return;
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workspaceView"></param>
        void INavigator.AddView(WorkspaceViewModelBase workspaceView)
        {
            if (null == workspaceView)
                throw new ArgumentNullException("workspaceView");
            _views.Add(workspaceView.RegisteredName, workspaceView);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="homeView"></param>
        void INavigator.AddHomeView(WorkspaceViewModelBase homeView)
        {
            _homeView = homeView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewKey"></param>
        void INavigator.NavigateToView(string viewKey)
        {
            if (_views.ContainsKey(viewKey))
            {
                _currentView =_views[viewKey];
                OnPropertyChanged("CurrentView");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void INavigator.NavigateToHome()
        {
            NavigateToHome();
        }

        /// <summary>
        /// 
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorkspaceViewModelBase> INavigator.GetAllView()
        {
            return _views.Keys.Select(key => _views[key]).ToList();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="Navigator"/>.
        /// </summary>
        public Navigator()
        {
            _views = new Dictionary<string, WorkspaceViewModelBase>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raise the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName"></param>
        private void OnPropertyChanged(string propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Sets the current view to home view.
        /// </summary>
        private void NavigateToHome()
        {
            _currentView = _homeView;
            OnPropertyChanged("CurrentView");
        }

        #endregion

        #region Member Variables

        private PropertyChangedEventHandler _propertyChanged;
        private readonly IDictionary<string, WorkspaceViewModelBase> _views;
        private WorkspaceViewModelBase _homeView;
        private WorkspaceViewModelBase _currentView;

        #endregion

    }
}
