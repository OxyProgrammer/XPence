/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System.Collections.ObjectModel;

namespace XPence.Infrastructure.BaseClasses
{
    /// <summary>
    /// A contract to be implemented by the view model instance that caters to the shell view.
    /// This contact is used by the shell to get the flyouts.
    /// </summary>
    public interface IFlyoutContainer
    {
        /// <summary>
        /// A collection of <see cref="FlyoutViewModelBase"/> instances.
        /// </summary>
        ObservableCollection<FlyoutViewModelBase> Flyouts { get; set; }
    }
}
