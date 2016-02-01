/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System.Windows;
using XPence.Infrastructure.BaseClasses;

namespace XPence.AttachedMembers
{
    /// <summary>
    /// Provides attachted properties for loading of an element.
    /// </summary>
    public static class ElementLoadingBehavior
    {
        #region InitializeDataContextWhenLoaded

        /// <summary>
        /// Gets the value of <see cref="InitializeDataContextWhenLoadedProperty"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns><see langword="bool"/></returns>
        public static bool GetInitializeDataContextWhenLoaded(FrameworkElement element)
        {
            return (bool)element.GetValue(InitializeDataContextWhenLoadedProperty);
        }

        /// <summary>
        /// sets the value of <see cref="InitializeDataContextWhenLoadedProperty"/>.
        /// </summary>
        /// <param name="element"></param>
        public static void SetInitializeDataContextWhenLoaded(
          FrameworkElement element, bool value)
        {
            element.SetValue(InitializeDataContextWhenLoadedProperty, value);
        }

        /// <summary>
        /// The attached property that attaches the behaviour to a <see cref="FrameworkElement"/>.
        /// </summary>
        public static readonly DependencyProperty InitializeDataContextWhenLoadedProperty =
            DependencyProperty.RegisterAttached(
            "InitializeDataContextWhenLoaded",
            typeof(bool),
            typeof(ElementLoadingBehavior),
            new UIPropertyMetadata(false, OnInitializeDataContextWhenLoadedChanged));

        /// <summary>
        /// Property changed event handler for <see cref="InitializeDataContextWhenLoadedProperty"/>.
        /// </summary>
        /// <param name="depObj"></param>
        /// <param name="e"></param>
        static void OnInitializeDataContextWhenLoadedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var item = depObj as FrameworkElement;
            if (item == null)
                return;
            if (e.NewValue is bool == false)
                return;
            if ((bool)e.NewValue)
                item.Loaded += OnElementLoaded;
            else
                item.Loaded -= OnElementLoaded;
        }

        /// <summary>
        /// The event handler to handle the loaded event of a framework element.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (!ReferenceEquals(sender, e.OriginalSource))
                return;
            var item = e.OriginalSource as FrameworkElement;
            if (item != null)
            {
                var dataContext = item.DataContext as ViewModelBase;
                if(null!=dataContext && !dataContext.IsInitialized)
                {
                    dataContext.Initialize();
                }
            }
        }

        #endregion
    }
}
