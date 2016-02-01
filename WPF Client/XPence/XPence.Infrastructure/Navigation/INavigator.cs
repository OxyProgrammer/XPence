/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using XPence.Infrastructure.BaseClasses;

namespace XPence.Infrastructure.Navigation
{
    /// <summary>
    /// A contract for an instance of navigator.
    /// Provides the members required for navigation.
    /// </summary>
    public interface INavigator:INotifyPropertyChanged
    {
        /// <summary>
        /// Commands the navigator instance to navigate back.
        /// </summary>
        void NavigateBack();

        /// <summary>
        /// Commands the navigator instance to navigate back.
        /// </summary>
        void NavigateToHome();

        /// <summary>
        /// Gets or sets the current selected view.
        /// </summary>
        WorkspaceViewModelBase CurrentView { get; set; }

        /// <summary>
        /// Gets the Enumerable of all the views.
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorkspaceViewModelBase> GetAllView();

        /// <summary>
        /// Adds a workspace view to the navigator instance.
        /// </summary>
        /// <param name="workspaceView"></param>
        void AddView(WorkspaceViewModelBase workspaceView);

        /// <summary>
        /// Adds the home view model.
        /// </summary>
        /// <param name="workspaceView"></param>
        void AddHomeView(WorkspaceViewModelBase workspaceView);

        /// <summary>
        /// Navigates to the view specified by the key.
        /// </summary>
        /// <param name="viewKey"></param>
        void NavigateToView(string viewKey);
    }
}
