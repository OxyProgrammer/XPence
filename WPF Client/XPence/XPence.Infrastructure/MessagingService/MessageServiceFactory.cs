/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

namespace XPence.Infrastructure.MessagingService
{
    /// <summary>
    /// A messaging service factory class to get the singleton instance of <see cref="IMessagingService"/>.
    /// </summary>
    public static class MessageServiceFactory
    {
        #region Public static Methods

        /// <summary>
        /// Gets a singleton instance of <see cref="IMessagingService"/>.
        /// </summary>
        /// <returns></returns>
        public static IMessagingService GetMessagingServiceInstance()
        {
            if (null == _messagingServiceInstance)
            {
                lock (_syncObject)
                {
                    if (null == _messagingServiceInstance)
                    {
                        _messagingServiceInstance = new MessagingService();
                    }
                }
            }
            return _messagingServiceInstance;
        }

        #endregion

        #region Member Variables

        private static volatile IMessagingService _messagingServiceInstance;
        private static readonly object _syncObject = new object();

        #endregion
    }
}
