using System;
using XPence.Models.DataModels;

namespace XPence.Models.EventArgsAndException
{
    /// <summary>
    /// An event args class to carry the user information on successful logged in event.
    /// </summary>
    public class UserLoggedInEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the LoggedInUser info.
        /// </summary>
        public User LoggedInUser { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="UserLoggedInEventArgs"/>.
        /// </summary>
        /// <param name="user">The logge in user model.</param>
        public UserLoggedInEventArgs(User user)
        {
            LoggedInUser = user;
        }

        #endregion


    }
}
