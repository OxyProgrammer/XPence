/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace XPence.Infrastructure.MessagingService
{
    /// <summary>
    /// Registers the modal views used in the application against a key.
    /// This is a singleton class. Get an instance of the class at the application startup and
    /// Register all the modal views against suitable keys.
    /// This key would be used later on by the <see cref="IMessagingService"/> instance to extract the view and display in a modal dialog.
    /// </summary>
    public class ModalViewRegistry
    {
        #region Public Members

        /// <summary>
        /// Gets the sole instance of the MainViewRegistry class.
        /// </summary>
        public static ModalViewRegistry Instance { get { return _instance.Value; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a UserControl that needs to be shown as a modal dialog.
        /// </summary>
        /// <param name="key">Key against which the user control is stored in the registry.</param>
        /// <param name="userControlType">The type of UserControl that would be shown as a modal dialog.</param>
        /// <exception cref="InvalidOperationException">Throws exception if an existing key is used to add another UserControl. 
        /// This is to eliminate the risk of overwriting an existing UserControl in the registry.</exception>
        public void RegisterView(string key, Type userControlType)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key");
            if (null == userControlType)
                throw new ArgumentException("userControl");
            if (!userControlType.IsSubclassOf(typeof(UserControl)))
                throw new InvalidCastException("Only a user control type can be assigned.");
            if (_modalViewRegistry.ContainsKey(key))
                throw new InvalidOperationException("Key already exists.");
            _modalViewRegistry[key] = userControlType;
        }

        /// <summary>
        /// Determines if a  key is already registered in the registry.
        /// </summary>
        /// <param name="viewKey">The key that needs for checked in the registry.</param>
        /// <returns></returns>
        public bool ContainsKey(string viewKey)
        {
            return _modalViewRegistry.ContainsKey(viewKey);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the registered UserControl against the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal UserControl GetViewByKey(string key)
        {
            Type userControlType = _modalViewRegistry[key];
            return Activator.CreateInstance(userControlType) as UserControl;
        }

        #endregion

        #region Member Variables

        private readonly IDictionary<string, Type> _modalViewRegistry;
        // static holder for instance, need to use lambda to construct since constructor is private.
        private static readonly Lazy<ModalViewRegistry> _instance
          = new Lazy<ModalViewRegistry>(() => new ModalViewRegistry());
        #endregion

        #region Contructor

        /// <summary>
        /// Initializes and instance of <see cref="MessagingService"/>
        /// </summary>
        private ModalViewRegistry()
        {
            _modalViewRegistry = new Dictionary<string, Type>();
        }

        #endregion
    }
}
