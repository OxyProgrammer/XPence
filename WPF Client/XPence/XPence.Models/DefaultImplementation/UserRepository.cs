using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NHibernate;
using NHibernate.Criterion;
using XPence.Infrastructure.Utility;
using XPence.Models.DataModels;
using XPence.Models.EventArgsAndException;
using XPence.Models.Interfaces;
using XPence.Shared;

namespace XPence.Models.DefaultImplementation
{
    /// <summary>
    /// An internal default implementation of <see cref="IUserRepository"/>.
    /// </summary>
    internal class UserRepository : IUserRepository
    {
        #region Implementation of IUserRepository

        /// <summary>
        /// Gets a list of <see cref="User"/> instances.
        /// </summary>
        /// <returns>A list of <see cref="User"/> <see langword="null"/> if none are there.</returns>
        IList<User> IUserRepository.GetAllUsers()
        {
            return GetAllUsers();
        }

        /// <summary>
        /// Gets all users asynchronously.
        /// <remarks>If this method is consumed, make sure to hook to the <seealso cref="IUserRepository.GetAllUsersCompleted"/> event of this instance of <see cref="IUserRepository"/>.</remarks>
        /// </summary>
        void IUserRepository.GetAllUsersAsync()
        {
            QueueAsyncTask(() =>
            {
                Exception exception = null;
                string message = null;
                IList<User> userList = null;
                try
                {
                    //This treachery is for faking a web service call.
                    Thread.Sleep(2000);
                    userList = GetAllUsers();
                    LogUtil.LogInfo("UserRepository", "IUserRepository.GetAllUsersAsync", "Successfully acquired the list.");
                }
                catch (Exception ex)
                {
                    LogUtil.LogError("UserRepository", "IUserRepository.GetAllUsersAsync", ex);
                    exception = ex;
                    message = ErrorMessages.ERR_FAILED_TO_LOAD_USERS;
                }
                finally
                {
                    if (null != _getAllUsersCompleted)
                        _getAllUsersCompleted(this, new GetAllUsersTasskFinishedEventArg(exception, message, userList));
                }
            });
        }

        /// <summary>
        /// An event that gets fired when the <seealso cref="GetAllUsersAsync"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<GetAllUsersTasskFinishedEventArg> IUserRepository.GetAllUsersCompleted
        {
            add { _getAllUsersCompleted += value; }
            remove { _getAllUsersCompleted -= value; }
        }

        /// <summary>
        /// Saves a <see cref="User"/> model in persistence database.
        /// </summary>
        /// <param name="user">The <see cref="User"/> instance that needs to be saved in the persistence database.</param>
        void IUserRepository.SaveUser(User user)
        {
            SaveUser(user);
        }

        /// <summary>
        /// Saves a <see cref="User"/> model in persistent database asynchronously.
        /// <remarks>
        /// If this method is consumed, make sure to hook to the <seealso cref="IUserRepository.SaveUserCompleted"/> event of this instance of <see cref="IUserRepository"/>.
        /// </remarks>
        /// </summary>
        /// <param name="user">The <see cref="User"/> instance that needs to be saved in the persistent database.</param>
        void IUserRepository.SaveUserAsync(User user)
        {
            QueueAsyncTask(() =>
            {
                Exception exception = null;
                string message = null;
                try
                {
                    //This treachery is for faking a web service call.
                    Thread.Sleep(2000);
                    SaveUser(user);
                    LogUtil.LogInfo("UserRepository",
                                    "IUserRepository.SaveUser",
                                    string.Format("Successfully saved the user{0}.", user.UserId));
                }
                catch (Exception ex)
                {
                    LogUtil.LogError("UserRepository",
                                     "IUserRepository.SaveUser", ex);
                    exception = ex;
                    message = ErrorMessages.ERR_FAILED_TO_SAVE_USER;
                }
                finally
                {
                    if (null != _saveUserCompleted)
                        _saveUserCompleted(this, new RepositoryTaskFinishedEventArgs(exception, message));
                }
            });
        }

        /// <summary>
        /// An event that gets fired when the <seealso cref="IUserRepository.SaveUserCompleted"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<RepositoryTaskFinishedEventArgs> IUserRepository.SaveUserCompleted
        {
            add { _saveUserCompleted += value; }
            remove { _saveUserCompleted -= value; }
        }

        /// <summary>
        /// Gets a new instance of <see cref="User"/>.
        /// </summary>
        /// <returns>An instance of <see cref="User"/>.</returns>
        User IUserRepository.GetNewUser()
        {
            return new User()
                       {
                           Role = UserRole.Normal,
                           SelectedAccent = AppearanceManager.GetApplicationAccent(),
                           SelectedTheme = AppearanceManager.GetApplicationTheme(),
                           Password = "XPense123"
                       };
        }

        /// <summary>
        /// Checks if a username is already existing in the db.
        /// </summary>
        /// <param name="username">The username that neds to be checked.</param>
        /// <returns>Returns <see langword="true"/> if username exists in the db. Returns <see langword="false"/> otherwise.</returns>
        bool IUserRepository.CheckIfUserNameExists(string username)
        {
            LogUtil.LogInfo("UserRepository",
                                   "IUserRepository.CheckIfUserNameExists",
                                   string.Format("Checked for username: {0}", username));
            return CheckIfUserNameExists(username);
        }

        /// <summary>
        /// Gets the user who attempts to login.
        /// </summary>
        /// <param name="username">Username as supplied by the user.</param>
        /// <param name="password">Password as supplied by the user.</param>
        /// <returns>An instance of <see cref="User"/> if the attempt succeeds. Returns null otherwise. </returns>
        User IUserRepository.ValidateLoginAttempt(string username, string password)
        {
            LogUtil.LogInfo("UserRepository",
                                   "IUserRepository.ValidateLoginAttempt",
                                   string.Format("Login attempted for  username: {0}.", username));
            return ValidateLoginAttempt(username, password);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the user that attempts login.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private User ValidateLoginAttempt(string username, string password)
        {
            string encryptedPwd = Util.Encrypt(password);
            using (var session = SessionProvider.SessionFactory.OpenSession())
            {
                //Build the criteria.
                var criteria = session.CreateCriteria<User>();
                criteria.Add(Restrictions.Eq("Username", username));
                criteria.Add(Restrictions.Eq("EncryptedPassword", encryptedPwd));
                var resultList = criteria.List<User>();
                if (null == resultList || resultList.Count < 1)
                    return null;
                else
                {
                    return resultList[0];
                }
            }
        }

        /// <summary>
        /// Gets the list of all users from the persistence database.
        /// </summary>
        /// <returns></returns>
        private IList<User> GetAllUsers()
        {
            using (var session = SessionProvider.SessionFactory.OpenSession())
            {
                //Build the criteria.
                ICriteria criteria = session.CreateCriteria<User>();
                return criteria.List<User>();
            }
        }

        /// <summary>
        /// Saves the user in the persistent db.
        /// </summary>
        /// <param name="user"></param>
        private void SaveUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            using (var session = SessionProvider.SessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    session.SaveOrUpdate(user);
                    trans.Commit();
                }
            }
        }

        /// <summary>
        /// Checks if a username is duplicate.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private bool CheckIfUserNameExists(string userName)
        {
            //I am sure there  must be a better way.
            // Too laxy to learn NHibernate at the moment.
            using (var session = SessionProvider.SessionFactory.OpenSession())
            {
                //Build the criteria.
                var criteria = session.CreateCriteria<User>();
                criteria.Add(Restrictions.Eq("Username", userName));
                var result = criteria.List<User>();
                return null != result && result.Count != 0;
            }
        }

        /// <summary>
        /// A private helper method that queues a method to a thread pool thread.
        /// </summary>
        /// <param name="method">The method that needs to be queued.</param>
        private void QueueAsyncTask(Action method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            ThreadPool.QueueUserWorkItem((o) => method());
        }

        #endregion

        #region Member Variables

        private event EventHandler<GetAllUsersTasskFinishedEventArg> _getAllUsersCompleted;
        private event EventHandler<RepositoryTaskFinishedEventArgs> _saveUserCompleted;

        #endregion
    }
}
