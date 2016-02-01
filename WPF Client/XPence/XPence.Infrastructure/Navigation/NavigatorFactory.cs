/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

namespace XPence.Infrastructure.Navigation
{
    /// <summary>
    /// A factory class to provide the singleton instance of <see cref="INavigator"/>.
    /// </summary>
    public static class NavigatorFactory
    {
        #region Public Methods.

        /// <summary>
        /// Gets the singleton instance of <see cref="INavigator"/>.
        /// </summary>
        /// <returns></returns>
        public static INavigator GetNavigator()
        {
            if(_navigator==null)
            {
                lock (_syncObject)
                {
                    if (_navigator == null)
                    {
                        _navigator = new Navigator();
                    }
                }
            }
            return _navigator;
        }

        #endregion

        #region Static constructor

        /// <summary>
        /// Initializes the static variables of the class.
        /// </summary>
        static NavigatorFactory()
        {
            _syncObject = new object();
        }

        #endregion


        #region Member Variables

        private static INavigator _navigator;
        private static object _syncObject;

        #endregion

    }
}
