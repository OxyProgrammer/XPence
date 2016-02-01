using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NHibernate;
using NHibernate.Criterion;
using XPence.Infrastructure.CoreClasses;
using XPence.Infrastructure.Utility;
using XPence.Models.DataModels;
using XPence.Models.EventArgsAndException;
using XPence.Models.Interfaces;
using XPence.Shared;

namespace XPence.Models.DefaultImplementation
{
    /// <summary>
    /// An internal default implementation of <see cref="ITransactionRepository"/>
    /// </summary>
    internal class TransactionRepository : ITransactionRepository
    {
        #region Implementation of ITransactionRepository

        /// <summary>
        /// Gets a list of <see cref="Transaction"/> instances.
        /// </summary>
        /// <param name="filter">The instance of <see cref="TransactionFilter"/> that wraps all filter values.</param>
        /// <returns>A list of transactions <see langword="null"/> if none are there.</returns>
        IList<Transaction> ITransactionRepository.GetTransactions(TransactionFilter filter)
        {
            return GetTransactions(filter);
        }

        /// <summary>
        /// Gets the filtered transactions ssynchronously.
        /// <remarks>If this method is consumed, make sure to hook to the <seealso cref="ITransactionRepository.GetTransactionsCompleted"/> event of this instance of <see cref="ITransactionRepository"/>.</remarks>
        /// <param name="filter">The instance of <see cref="TransactionFilter"/> that wraps all filter values.</param>
        /// </summary>
        void ITransactionRepository.GetTransactionsAsync(TransactionFilter filter)
        {
            QueueAsyncTask(() =>
            {
                Exception exception = null;
                string message = null;
                IList<Transaction> transList = null;
                try
                {
                    //This treachery is for faking a web service call.
                    Thread.Sleep(2000);
                    transList = GetTransactions(filter);
                    LogUtil.LogInfo("TransactionRepository", "ITransactionRepository.GetTransactionsAsync", "Successfully acquired the list.");
                }
                catch (Exception ex)
                {
                    LogUtil.LogError("TransactionRepository", "ITransactionRepository.GetTransactionsAsync", ex);
                    exception = ex;
                    message = ErrorMessages.ERR_FAILED_TO_LOAD_TRNS;
                }
                finally
                {
                    if (null != _getTransactionsCompleted)
                        _getTransactionsCompleted(this, new GetTransactionFinishedEventArg(exception, message, transList));
                }
            });
        }

        /// <summary>
        /// An event that gets fired when the <seealso cref="ITransactionRepository.GetTransactionsAsync"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<GetTransactionFinishedEventArg> ITransactionRepository.GetTransactionsCompleted
        {
            add { _getTransactionsCompleted += value; }
            remove { _getTransactionsCompleted -= value; }
        }

        /// <summary>
        /// Saves a <see cref="Transaction"/> model in persistence database.
        /// </summary>
        /// <param name="transaction">The transaction tha needs to be saved to the persistent DB.</param>
        void ITransactionRepository.SaveTransaction(Transaction transaction)
        {
            SaveTransaction(transaction);
        }

        /// <summary>
        /// Saves a <see cref="Transaction"/> model in persistence database asynchronously.
        /// <remarks>
        /// If this method is consumed, make sure to hook to the <seealso cref="ITransactionRepository.SaveTransactionCompleted"/> event of this instance of <see cref="ITransactionRepository"/>.
        /// </remarks>
        /// </summary>
        /// <param name="transaction">The <see cref="Transaction"/> that needs to be saved in the persistence database.</param>
        void ITransactionRepository.SaveTransactionAsync(Transaction transaction)
        {
            QueueAsyncTask(() =>
            {
                Exception exception = null;
                string message = null;
                try
                {
                    //This treachery is for faking a web service call.
                    Thread.Sleep(2000);
                    SaveTransaction(transaction);
                    LogUtil.LogInfo("TransactionRepository",
                                    "ITransactionRepository.SaveTransactionAsync",
                                    string.Format("Successfully saved the trans: {0}.", transaction.TransactionId));
                }
                catch (Exception ex)
                {
                    LogUtil.LogError("TransactionRepository",
                                     "ITransactionRepository.SaveTransactionAsync", ex);
                    exception = ex;
                    message = ErrorMessages.ERR_FAILED_TO_SAVE_TRANS;
                }
                finally
                {
                    if (null != _saveTransactionCompleted)
                        _saveTransactionCompleted(this,
                                                  new RepositoryTaskFinishedEventArgs(exception, message));
                }
            });
        }

        /// <summary>
        /// An event that gets fired when the <seealso cref="ITransactionRepository.SaveTransactionAsync"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<RepositoryTaskFinishedEventArgs> ITransactionRepository.SaveTransactionCompleted
        {
            add { _saveTransactionCompleted += value; }
            remove { _saveTransactionCompleted -= value; }
        }

        /// <summary>
        /// Deletes transactions from the persistent database.
        /// </summary>
        /// <param name="transactions">The array of all <see cref="Transaction"/> instances that need to
        ///  be deleted from the persistence database.</param>
        void ITransactionRepository.DeleteTransactions(Transaction[] transactions)
        {
            DeleteTransactions(transactions);
        }

        /// <summary>
        /// Deletes transactions from the persistent database asynchronously.
        /// <remarks>
        /// If this method is consumed, make sure to hook to the <seealso cref="ITransactionRepository.DeleteTransactionsCompleted"/> event of this instance of <see cref="ITransactionRepository"/>.
        /// </remarks>
        /// </summary>
        /// <param name="transactions">The array of all <see cref="Transaction"/> instances that need to
        ///  be deleted from the persistence database.</param>
        void ITransactionRepository.DeleteTransactionsAsync(Transaction[] transactions)
        {
            QueueAsyncTask(() =>
            {
                Exception exception = null;
                string message = null;
                IList<long> transIds=null;
                try
                {
                    //This treachery is for faking a web service call.
                    Thread.Sleep(2000);
                    DeleteTransactions(transactions);
                    LogUtil.LogInfo("TransactionRepository",
                                    "ITransactionRepository.DeleteTransactionsAsync",
                                    "Successfully deleted the trans.");
                    transIds=transactions.Select(t => t.TransactionId).ToList();

                }
                catch (Exception ex)
                {
                    LogUtil.LogError("TransactionRepository",
                                     "ITransactionRepository.DeleteTransactionsAsync", ex);
                    exception = ex;
                    message = ErrorMessages.ERR_FAILED_TO_DELETE_TRANS;
                }
                finally
                {
                    if (null != _deleteTransactionsCompleted)
                        _deleteTransactionsCompleted(this, new DeleteTransactionsFinishedEventArg(exception, message, transIds));
                }
            });
        }

        /// <summary>
        /// An event that gets fired when the <seealso cref="ITransactionRepository.DeleteTransactionsAsync"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<DeleteTransactionsFinishedEventArg> ITransactionRepository.DeleteTransactionsCompleted
        {
            add { _deleteTransactionsCompleted += value; }
            remove { _deleteTransactionsCompleted -= value; }
        }

        /// <summary>
        /// Gets a new <see cref="Transaction"/> instance.
        /// </summary>
        /// <returns></returns>
        Transaction ITransactionRepository.GetNewTransaction()
        {
            var transaction = new Transaction {TransactionDate = DateTime.Now.Date};
            return transaction;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the transactions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IList<Transaction> GetTransactions(TransactionFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");
            using (var session = SessionProvider.SessionFactory.OpenSession())
            {
                //Build the criteria.
                ICriteria criteria = session.CreateCriteria<Transaction>();
                if (filter.FromDate.HasValue && filter.ToDate.HasValue)
                {
                    criteria.Add(Restrictions.Between("TransactionDate", filter.FromDate.Value, filter.ToDate.Value));
                }
                if (filter.FromAmount.HasValue && filter.ToAmount.HasValue)
                {
                    criteria.Add(Restrictions.Between("Amount", filter.FromAmount.Value, filter.ToAmount.Value));
                }
                if (!string.IsNullOrEmpty(filter.Username))
                {
                    criteria.Add(Restrictions.Eq("CreatedBy", filter.Username));
                }
                return criteria.List<Transaction>();
            }
        }

        /// <summary>
        /// Saves a <see cref="Transaction"/> model in persistence database.
        /// </summary>
        /// <param name="transaction">The transaction tha needs to be saved to the persistent DB.</param>
        private void SaveTransaction(Transaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            using (var session = SessionProvider.SessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    session.SaveOrUpdate(transaction);
                    trans.Commit();
                }
            }
        }

        /// <summary>
        /// Deletes transactions from the persistent database.
        /// </summary>
        /// <param name="transactions">The array of all <see cref="Transaction"/> instances that need to
        ///  be deleted from the persistence database.</param>
        private void DeleteTransactions(Transaction[] transactions)
        {
            if (transactions == null)
                throw new ArgumentNullException("transactions");
            using (var session = SessionProvider.SessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    transactions.ForEach(transaction => session.Delete(transaction));
                    trans.Commit();
                }
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

        private event EventHandler<GetTransactionFinishedEventArg> _getTransactionsCompleted;
        private event EventHandler<DeleteTransactionsFinishedEventArg> _deleteTransactionsCompleted;
        private event EventHandler<RepositoryTaskFinishedEventArgs> _saveTransactionCompleted;

        #endregion
    }
}
