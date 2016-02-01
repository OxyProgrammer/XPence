using XPence.Models.DefaultImplementation;
using XPence.Models.Interfaces;

namespace XPence.Models
{
    /// <summary>
    /// A factory class for returning default implementation of repostory contracts.
    /// </summary>
    public static class RepositoryFactory
    {
        #region Public static Methods

        /// <summary>
        /// Gets a singleton instance of Transaction repository.
        /// </summary>
        /// <returns></returns>
        public static ITransactionRepository GetTransactionRepository()
        {
            if (null == _transactionRepositoryInstance)
            {
                lock (_syncObject)
                {
                    if (null == _transactionRepositoryInstance)
                    {
                        _transactionRepositoryInstance = new TransactionRepository();
                    }
                }
            }
            return _transactionRepositoryInstance;
        }

        /// <summary>
        /// Gets a singleton instance of User repository.
        /// </summary>
        /// <returns></returns>
        public static IUserRepository GetUserRepository()
        {
            if (null == _userRepositoryInstance)
            {
                lock (_syncObject)
                {
                    if (null == _userRepositoryInstance)
                    {
                        _userRepositoryInstance = new UserRepository();
                    }
                }
            }
            return _userRepositoryInstance;
        }

        #endregion

        #region Member Variables

        private static volatile ITransactionRepository _transactionRepositoryInstance;
        private static readonly object _syncObject = new object();
        private static volatile IUserRepository  _userRepositoryInstance ;

        #endregion
    }
}
