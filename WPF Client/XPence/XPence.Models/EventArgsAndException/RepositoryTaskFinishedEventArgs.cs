using System;
using System.Collections.Generic;
using XPence.Models.DataModels;

namespace XPence.Models.EventArgsAndException
{
    /// <summary>
    /// An event args class to carry the data of a repository task finised event.
    /// </summary>
    public class RepositoryTaskFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the exception, if any, during task finish.
        /// </summary>
        public Exception TaskFailureException { get; private set; }

        /// <summary>
        /// Gets if there has been any exception in the task finished
        /// </summary>
        public bool HasError
        {
            get { return null != TaskFailureException; }
        }

        /// <summary>
        /// Gets a user friendly message about the task completion.
        /// </summary>
        public string FinishMessage { get; private set; }

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="RepositoryTaskFinishedEventArgs"/>
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> that has occurred. Send <see langword="null"/> if nonw occured. </param>
        /// <param name="finishMessage">A user friendly message that will be shown by the consumer to the user.</param>
        public RepositoryTaskFinishedEventArgs(Exception exception, string finishMessage)
        {
            FinishMessage = finishMessage;
            TaskFailureException = exception;
        }

        #endregion

    }

    /// <summary>
    /// An event args class to carry the ids of the deleted transactions.
    /// </summary>
    public class DeleteTransactionsFinishedEventArg:RepositoryTaskFinishedEventArgs
    {
        #region Public properties

        /// <summary>
        /// Gets the list of the transactions that are deleted.
        /// </summary>
        public IList<long> DeletedTransactionsIdList { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="DeleteTransactionsFinishedEventArg"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> that has occurred. Send <see langword="null"/> if none occured. </param>
        /// <param name="finishMessage">A user friendly message that will be shown by the consumer to the user.</param>
        /// <param name="deletedList">The list of <see cref="Transaction"/> ids that got deleted.</param>
        public DeleteTransactionsFinishedEventArg(Exception exception, string finishMessage, IList<long> deletedList)
            : base(exception, finishMessage)
        {
            DeletedTransactionsIdList = deletedList;
        }

        #endregion

    }

    /// <summary>
    /// Carries the data of TransactionFinished event.
    /// </summary>
    public class GetTransactionFinishedEventArg : RepositoryTaskFinishedEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Gets the transaction list.
        /// </summary>
        public IList<Transaction> TransactionList { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="GetTransactionFinishedEventArg"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> that has occurred. Send <see langword="null"/> if none occured. </param>
        /// <param name="finishMessage">A user friendly message that will be shown by the consumer to the user.</param>
        /// <param name="list">The list of <see cref="Transaction"/> instances.</param>
        public GetTransactionFinishedEventArg(Exception exception, string finishMessage, IList<Transaction> list)
            : base(exception, finishMessage)
        {
            TransactionList = list;
        }

        #endregion
    }

    /// <summary>
    /// Gets the data of GetAllUsrs finished event.
    /// </summary>
    public class GetAllUsersTasskFinishedEventArg:RepositoryTaskFinishedEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Gets the transaction list.
        /// </summary>
        public IList<User> UserList { get; private set; }

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="GetTransactionFinishedEventArg"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> that has occurred. Send <see langword="null"/> if none occured. </param>
        /// <param name="finishMessage">A user friendly message that will be shown by the consumer to the user.</param>
        /// <param name="list">The list of <see cref="User"/> instances.</param>
        public GetAllUsersTasskFinishedEventArg(Exception exception, string finishMessage, IList<User> list)
            : base(exception, finishMessage)
        {
            UserList = list;
        }

        #endregion
    }
}
