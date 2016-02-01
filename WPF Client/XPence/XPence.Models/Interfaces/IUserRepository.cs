using System;
using System.Collections.Generic;
using XPence.Models.DataModels;
using XPence.Models.EventArgsAndException;

namespace XPence.Models.Interfaces
{
    /// <summary>
    /// A contract exposing all members essential for maintaining a repository of 
    /// <see cref="Transaction"/> instances.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a list of <see cref="User"/> instances.
        /// </summary>
        /// <returns>A list of <see cref="User"/> <see langword="null"/> if none are there.</returns>
        IList<User> GetAllUsers();

        /// <summary>
        /// Gets all users asynchronously.
        /// <remarks>If this method is consumed, make sure to hook to the <seealso cref="GetAllUsersCompleted"/> event of this instance of <see cref="IUserRepository"/>.</remarks>
        /// </summary>
        void GetAllUsersAsync();

        /// <summary>
        /// An event that gets fired when the <seealso cref="GetAllUsersAsync"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<GetAllUsersTasskFinishedEventArg> GetAllUsersCompleted;

        /// <summary>
        /// Saves a <see cref="User"/> model in persistence database.
        /// </summary>
        /// <param name="user">The <see cref="User"/> instance that needs to be saved in the persistence database.</param>
        void SaveUser(User user);

        /// <summary>
        /// Saves a <see cref="User"/> model in persistent database asynchronously.
        /// <remarks>
        /// If this method is consumed, make sure to hook to the <seealso cref="SaveUserCompleted"/> event of this instance of <see cref="IUserRepository"/>.
        /// </remarks>
        /// </summary>
        /// <param name="user">The <see cref="User"/> instance that needs to be saved in the persistent database.</param>
        void SaveUserAsync(User user);

        /// <summary>
        /// An event that gets fired when the <seealso cref="SaveUserCompleted"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<RepositoryTaskFinishedEventArgs> SaveUserCompleted;

        /// <summary>
        /// Gets a new instance of <see cref="User"/>.
        /// </summary>
        /// <returns>An instance of <see cref="User"/>.</returns>
        User GetNewUser();

        /// <summary>
        /// Checks if a username is already existing in the db.
        /// </summary>
        /// <param name="username">The username that neds to be checked.</param>
        /// <returns>Returns <see langword="true"/> if username exists in the db. Returns <see langword="false"/> otherwise.</returns>
        bool CheckIfUserNameExists(string username);

        /// <summary>
        /// Gets the user who attempts to login.
        /// </summary>
        /// <param name="username">Username as supplied by the user.</param>
        /// <param name="password">Password as supplied by the user.</param>
        /// <returns>An instance of <see cref="User"/> if the attempt succeeds. Returns null otherwise. </returns>
        User ValidateLoginAttempt(string username, string password);
    }
}
