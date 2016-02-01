using System;
using System.Collections.Generic;
using XPence.Models.EventArgsAndException;
using XPence.Models.DataModels;

namespace XPence.Models.Interfaces
{
    /// <summary>
    /// A contract exposing all members essential for maintaining a repository of 
    /// <see cref="Transaction"/> instances.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Gets a list of <see cref="Transaction"/> instances.
        /// </summary>
        /// <param name="filter">The instance of <see cref="TransactionFilter"/> that wraps all filter values.</param>
        /// <returns>A list of transactions <see langword="null"/> if none are there.</returns>
        IList<Transaction> GetTransactions(TransactionFilter filter);

        /// <summary>
        /// Gets the filtered transactions asynchronously.
        /// <remarks>If this method is consumed, make sure to hook to the <seealso cref="GetTransactionsCompleted"/> event of this instance of <see cref="ITransactionRepository"/>.</remarks>
        /// <param name="filter">The instance of <see cref="TransactionFilter"/> that wraps all filter values.</param>
        /// </summary>
        void GetTransactionsAsync(TransactionFilter filter);

        /// <summary>
        /// An event that gets fired when the <seealso cref="GetTransactionsAsync"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<GetTransactionFinishedEventArg> GetTransactionsCompleted;

        /// <summary>
        /// Saves a <see cref="Transaction"/> model in persistence database.
        /// </summary>
        /// <param name="transaction">The <see cref="Transaction"/> that needs to be saved in the persistence database.</param>
        void SaveTransaction(Transaction transaction);

        /// <summary>
        /// Saves a <see cref="Transaction"/> model in persistence database asynchronously.
        /// <remarks>
        /// If this method is consumed, make sure to hook to the <seealso cref="SaveTransactionCompleted"/> event of this instance of <see cref="ITransactionRepository"/>.
        /// </remarks>
        /// </summary>
        /// <param name="transaction">The <see cref="Transaction"/> that needs to be saved in the persistence database.</param>
        void SaveTransactionAsync(Transaction transaction);

        /// <summary>
        /// An event that gets fired when the <seealso cref="SaveTransactionAsync"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<RepositoryTaskFinishedEventArgs> SaveTransactionCompleted;

        /// <summary>
        /// Deletes transactions from the persistent database.
        /// </summary>
        /// <param name="transactions">The array of all <see cref="Transaction"/> instances that need to
        ///  be deleted from the persistence database.</param>
        void DeleteTransactions(Transaction[] transactions);

        /// <summary>
        /// Deletes transactions from the persistent database asynchronously.
        /// <remarks>
        /// If this method is consumed, make sure to hook to the <seealso cref="DeleteTransactionsCompleted"/> event of this instance of <see cref="ITransactionRepository"/>.
        /// </remarks>
        /// </summary>
        /// <param name="transactions">The array of all <see cref="Transaction"/> instances that need to
        ///  be deleted from the persistence database.</param>
        void DeleteTransactionsAsync(Transaction[] transactions);

        /// <summary>
        /// An event that gets fired when the <seealso cref="DeleteTransactionsAsync"/> method finishes.
        /// <remarks>The event may get fired from a different thread than the UI thread.
        /// It is the consumer's responsibility to handle this event in a thread safe manner.</remarks>
        /// </summary>
        event EventHandler<DeleteTransactionsFinishedEventArg> DeleteTransactionsCompleted;

        /// <summary>
        /// Gets a new <see cref="Transaction"/> instance.
        /// </summary>
        /// <returns></returns>
        Transaction GetNewTransaction();
    }

    
}
