/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.CoreClasses;
using XPence.Infrastructure.MessagingService;
using XPence.Models.DataModels;
using XPence.Models.EventArgsAndException;
using XPence.Models.Interfaces;
using XPence.Shared;

namespace XPence.ViewModels
{
    /// <summary>
    /// A view model catering to the All Expenses view.
    /// </summary>
    public class AllTransactionViewModel : WorkspaceViewModelBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the transactions viewable by the users.
        /// </summary>
        public ExtendedObservableCollection<TransactionViewModel> Transactions
        {
            get { return _transactions; }
            set
            {
                if (value == _transactions)
                    return;
                _transactions = value;
                OnPropertyChanged(GetPropertyName(() => Transactions));
            }
        }

        /// <summary>
        /// Gets or sets the Selected transaction.
        /// </summary>
        public TransactionViewModel SelectedTransaction
        {
            get { return _selectedTransaction; }
            set
            {
                if (_selectedTransaction == value)
                    return;
                _selectedTransaction = value;
                OnPropertyChanged(GetPropertyName(() => SelectedTransaction));
            }
        }

        /// <summary>
        /// Gets the filter view model.
        /// </summary>
        public TransactionFilterViewModel FilterViewModel
        {
            get { return _filterViewModel; }
            private set
            {
                if (_filterViewModel == value)
                    return;
                _filterViewModel = value;
                OnPropertyChanged(GetPropertyName(() => FilterViewModel));
            }
        }

        /// <summary>
        /// Gets the total expenditure.
        /// </summary>
        public double TotalExpenditure
        {
            get { return _totalExpenditure; }
            private set
            {
                if (_totalExpenditure == value)
                    return;
                _totalExpenditure = value;
                OnPropertyChanged(GetPropertyName(() => TotalExpenditure));
            }
        }

        /// <summary>
        /// Gets the total income.
        /// </summary>
        public double TotalIncome
        {
            get { return _totalIncome; }
            private set
            {
                if (_totalIncome == value)
                    return;
                _totalIncome = value;
                OnPropertyChanged(GetPropertyName(() => TotalIncome));
            }
        }

        /// <summary>
        /// Gets or sets the Filter display string for filter on dates.
        /// </summary>
        public string FilterDisplayOnDates
        {
            get { return _filterDisplayOnDates; }
            private set
            {
                if (_filterDisplayOnDates == value)
                    return;
                _filterDisplayOnDates = value;
                OnPropertyChanged(GetPropertyName(() => FilterDisplayOnDates));

            }
        }

        /// <summary>
        /// Gets or sets the Filter display string for filter on Amount.
        /// </summary>
        public string FilterDisplayOnAmount
        {
            get { return _filterDisplayOnAmount; }
            private set
            {
                if (_filterDisplayOnAmount == value)
                    return;
                _filterDisplayOnAmount = value;
                OnPropertyChanged(GetPropertyName(() => FilterDisplayOnAmount));
            }
        }

        /// <summary>
        /// Gets or sets the Filter display string for filter on User.
        /// </summary>
        public string FilterDisplayOnUser
        {
            get { return _filterDisplayOnUser; }
            private set
            {
                if (_filterDisplayOnUser == value)
                    return;
                _filterDisplayOnUser = value;
                OnPropertyChanged(GetPropertyName(() => FilterDisplayOnUser));
            }
        }

        /// <summary>
        /// Gets or sets the GraphItems. This forms the datasource for the UI pirece responsible for rendering the graph.
        /// </summary>
        public ExtendedObservableCollection<GraphItemViewModel> GraphItems
        {
            get { return _graphItems; }
            set
            {
                if (_graphItems == value)
                    return;
                _graphItems = value;
                OnPropertyChanged(GetPropertyName(() => GraphItems));
            }
        }

        /// <summary>
        /// Gets if the logged in user is admin.
        /// </summary>
        public bool IsUserAdmin
        {
            get { return _isUserAdmin; }
            set
            {
                if (_isUserAdmin == value)
                    return;
                _isUserAdmin = value;
                OnPropertyChanged(GetPropertyName(() => IsUserAdmin));
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the command to save a transaction.
        /// </summary>
        public ICommand SaveTransactionCommand { get; private set; }

        /// <summary>
        /// Gets the command to create a new transaction.
        /// </summary>
        public ICommand AddNewTransactionCommand { get; private set; }

        /// <summary>
        /// Gets the command to delete transactions.
        /// </summary>
        public ICommand DeleteTransactionsCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="AllTransactionViewModel"/>.
        /// </summary>
        /// <param name="isUserAdmin">Gets if the logged in user is admin </param>
        /// <param name="registeredName">Registered name of this instance of <see cref="WorkspaceViewModelBase"/>.</param>
        /// <param name="userRepository"> </param>
        /// <param name="transactionRepository">An instance of transaction repository contract.</param>
        /// <param name="messagingService">An implementation of <see cref="IMessagingService"/>. </param>
        public AllTransactionViewModel(bool isUserAdmin, string registeredName, IUserRepository userRepository, ITransactionRepository transactionRepository, IMessagingService messagingService)
            : base(registeredName)
        {
            if (userRepository == null)
                throw new ArgumentNullException("userRepository");
            if (transactionRepository == null)
                throw new ArgumentNullException("transactionRepository");
            if (messagingService == null)
                throw new ArgumentNullException("messagingService");
            DisplayName = UIText.EXPENSE_VIEW_HEADER;
            _userRepository = userRepository;
            IsUserAdmin = isUserAdmin;
            _transactionRepository = transactionRepository;
            _messagingService = messagingService;
            CanGoBack = true;
            //Initialize commands
            AddNewTransactionCommand = new RelayCommand(() => SelectedTransaction = new TransactionViewModel(_transactionRepository.GetNewTransaction()));
            SaveTransactionCommand = new RelayCommand(SaveTransaction, CanSaveTransaction);
            DeleteTransactionsCommand = new RelayCommand(DeleteTransactions);
        }

        #endregion

        #region Private Event Handlers

        /// <summary>
        /// The handler handles the event when the <see cref="TransactionFilterViewModel.FilterApplied"/> event is raised
        /// by the <see cref="FilterViewModel"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterViewModelFilterApplied(object sender, EventArgs e)
        {
            SelectedTransaction = null;
            Transactions.Clear();
            _messagingService.ShowProgressMessage(UIText.WAIT_SCREEN_HEADER, UIText.GETTING_TRANS_WAIT_MSG);
            GetFilteredTransactions();
        }

        /// <summary>
        ///  This handler handles the event when the <see cref="ITransactionRepository.DeleteTransactionsCompleted"/> 
        /// event is raised by <see cref="_transactionRepository"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransactionRepositoryDeleteTransactionsCompleted(object sender, DeleteTransactionsFinishedEventArg e)
        {
            ThreadSafeInvoke(() => e.DeletedTransactionsIdList.ForEach(id =>
            {
                _messagingService.CloseProgressMessage();
                if (e.HasError)
                {
                    _messagingService.ShowMessage(UIText.ERROR_OCCURED_MSG);
                    return;
                }
                var trans = Transactions.FirstOrDefault(t => t.TransactionId == id);
                if (null != trans)
                    Transactions.Remove(trans);
                //RefreshGraph
                GraphItems = GetGraphData(Transactions.Select(t => t.Entity).ToList());
            }));
        }

        /// <summary>
        /// This handler handles the event when the <see cref="ITransactionRepository.SaveTransactionCompleted"/> 
        /// event is raised by <see cref="_transactionRepository"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransactionRepositorySaveTransactionCompleted(object sender, RepositoryTaskFinishedEventArgs e)
        {
            ThreadSafeInvoke(() =>
                                 {
                                     _messagingService.CloseProgressMessage();
                                     if (e.HasError)
                                     {
                                         _messagingService.ShowMessage(UIText.ERROR_OCCURED_MSG);
                                     }
                                     if (!Transactions.Contains(SelectedTransaction))
                                     {
                                         Transactions.Add(SelectedTransaction);
                                         if (null != SelectedTransaction)//Redundant but safe check
                                             SelectedTransaction.Refresh();
                                         TotalIncome = Transactions.Where(t => t.FlowType == TransactionFlowType.Income).Sum(t => t.Entity.Amount);
                                         TotalExpenditure = Transactions.Where(t => t.FlowType == TransactionFlowType.Expenditure).Sum(t => t.Entity.Amount);
                                         GraphItems = GetGraphData(Transactions.Select(t => t.Entity).ToList());
                                     }
                                 });
        }

        /// <summary>
        /// This handler handles the event when the <see cref="ITransactionRepository.GetTransactionsCompleted"/> 
        /// event is raised by <see cref="_transactionRepository"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransactionRepositoryGetTransactionsCompleted(object sender, GetTransactionFinishedEventArg e)
        {
            var graphData = GetGraphData(e.TransactionList);//Graph data is calculated in the background thread.
            double income = 0;
            double expense = 0;
            if (e.TransactionList != null)
            {
                income = e.TransactionList.Where(t => t.FlowType == TransactionFlowType.Income).Sum(t => t.Amount);
                expense = e.TransactionList.Where(t => t.FlowType == TransactionFlowType.Expenditure).Sum(t => t.Amount);
            }
            ThreadSafeInvoke(() =>
                                 {
                                     GraphItems = null;
                                     _messagingService.CloseProgressMessage();
                                     if (e.HasError)
                                     {
                                         _messagingService.ShowMessage(UIText.ERROR_OCCURED_MSG);
                                         return;
                                     }
                                     if (e.TransactionList == null || !e.TransactionList.Any())
                                     {
                                         _messagingService.ShowMessage(UIText.NO_DATA_EXISTS_MSG);
                                         return;
                                     }
                                     TotalExpenditure = expense;
                                     TotalIncome = income;
                                     var transactionList = e.TransactionList.Select(t => new TransactionViewModel(t));
                                     GraphItems = graphData;
                                     Transactions.AddRange(transactionList);
                                 });
        }

        #endregion

        #region Overriden Members

        /// <summary>
        /// Initialize the variables and wire up events.
        /// </summary>
        protected override void OnInitialize()
        {
            FilterViewModel = new TransactionFilterViewModel(IsUserAdmin, _userRepository, _messagingService)
            {
                Header = UIText.FILTER_HEADER_TEXT,
                Position = VisibilityPosition.Right,
                Theme = FlyoutTheme.AccentedTheme
            };
            //FilterViewModel.Initialize();
            _messagingService.RegisterFlyout(FilterViewModel);
            Transactions = new ExtendedObservableCollection<TransactionViewModel>();
            //register to the task completed events of the repository.
            _transactionRepository.GetTransactionsCompleted += TransactionRepositoryGetTransactionsCompleted;
            _transactionRepository.SaveTransactionCompleted += TransactionRepositorySaveTransactionCompleted;
            _transactionRepository.DeleteTransactionsCompleted += TransactionRepositoryDeleteTransactionsCompleted;
            //Register to the Filter view models applied filter event
            FilterViewModel.FilterApplied += FilterViewModelFilterApplied;
        }

        /// <summary>
        /// Clean up!
        /// </summary>
        protected override void OnDispose()
        {
            if (null != _transactionRepository)
            {
                _transactionRepository.GetTransactionsCompleted -= TransactionRepositoryGetTransactionsCompleted;
                _transactionRepository.SaveTransactionCompleted -= TransactionRepositorySaveTransactionCompleted;
                _transactionRepository.DeleteTransactionsCompleted -= TransactionRepositoryDeleteTransactionsCompleted;
            }
            if (null != FilterViewModel)
            {
                FilterViewModel.FilterApplied -= FilterViewModelFilterApplied;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets all the filtered transactions.
        /// </summary>
        private void GetFilteredTransactions()
        {
            string dateDisplayFilter = "N.A.";
            string fromAmountFilterDisplay = "0";
            string toAmountFilterDisplay = "max";

            var transactionFilter = new TransactionFilter();
            if (FilterViewModel.IsDateRangeIncluded)
            {
                transactionFilter.FromDate = FilterViewModel.FromDate;
                transactionFilter.ToDate = FilterViewModel.ToDate;
                dateDisplayFilter = string.Format("{0} - {1}", FilterViewModel.FromDate, FilterViewModel.ToDate);
            }

            if (null != FilterViewModel.FromAmount)
            {
                double amount = 0;
                if (double.TryParse(FilterViewModel.FromAmount, out amount))
                {
                    transactionFilter.FromAmount = amount;
                    fromAmountFilterDisplay = Convert.ToString(amount);
                }
            }
            if (null != FilterViewModel.ToAmount)
            {
                double amount = 0;
                if (double.TryParse(FilterViewModel.ToAmount, out amount))
                {
                    transactionFilter.ToAmount = amount;
                    toAmountFilterDisplay = Convert.ToString(amount);
                }
            }
            transactionFilter.Username = FilterViewModel.Username;
            if (!IsUserAdmin)//Override selected user name if user is not admin
                transactionFilter.Username = AppData.LoggedInUser.Username;
            //Update properties for the filter visual
            TotalIncome = 0;
            TotalExpenditure = 0;
            FilterDisplayOnDates = dateDisplayFilter;
            FilterDisplayOnAmount = string.Format("{0} - {1}", fromAmountFilterDisplay, toAmountFilterDisplay);
            FilterDisplayOnUser = FilterViewModel.Username ?? "N.A.";
            _transactionRepository.GetTransactionsAsync(transactionFilter);
        }

        /// <summary>
        /// Gets the graph data.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private ExtendedObservableCollection<GraphItemViewModel> GetGraphData(IList<Transaction> list)
        {
            ExtendedObservableCollection<GraphItemViewModel> graphItems = null;
            if (list != null && list.Any())
            {
                graphItems = new ExtendedObservableCollection<GraphItemViewModel>();
                var totalNeed = list.Where(t => t.PurposeType == TransactionPurposeType.Need).Sum(t => t.Amount);
                var totalWant = list.Where(t => t.PurposeType == TransactionPurposeType.Want).Sum(t => t.Amount);
                graphItems.Add(new GraphItemViewModel()
                {
                    Description = "Need",
                    Value = totalNeed
                });
                graphItems.Add(new GraphItemViewModel()
                {
                    Description = "Want",
                    Value = totalWant
                });
            }
            return graphItems;
        }

        /// <summary>
        /// Save the selected transction.
        /// </summary>
        private void SaveTransaction()
        {
            _messagingService.ShowProgressMessage(UIText.WAIT_SCREEN_HEADER, UIText.SAVING_TRANS_WAIT_MSG);
            if (SelectedTransaction.Entity.TransactionId < 1)
                SelectedTransaction.Entity.CreatedBy = AppData.LoggedInUser.Username;
            SelectedTransaction.Entity.LastModifiedBy = AppData.LoggedInUser.Username;
            _transactionRepository.SaveTransactionAsync(SelectedTransaction.Entity);
        }

        private bool CanSaveTransaction()
        {
            if (null == SelectedTransaction)
                return false;
            return SelectedTransaction.IsValid;
        }

        /// <summary>
        /// Deletes the transaction marked.
        /// If no transaction id marked, the selected transaction is deleted.
        /// </summary>
        private void DeleteTransactions()
        {
            if (Transactions.Any())
            {
                var markedTrans = Transactions.Where(t => t.IsMarked);
                if (markedTrans.Any())
                {
                    var markedArray = markedTrans.Select(t => t.Entity).ToArray();
                    _messagingService.ShowProgressMessage(UIText.WAIT_SCREEN_HEADER, UIText.DELETING_TRANS_WAIT_MSG);
                    _transactionRepository.DeleteTransactionsAsync(markedArray);
                    return;
                }
            }
            _messagingService.ShowMessage(InfoMessages.INF_MARK_FOR_DEL);
        }

        #endregion

        #region Member Variables

        private ExtendedObservableCollection<TransactionViewModel> _transactions;
        private TransactionViewModel _selectedTransaction;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMessagingService _messagingService;
        private string _filterDisplayOnDates;
        private string _filterDisplayOnAmount;
        private string _filterDisplayOnUser;
        private double _totalExpenditure;
        private double _totalIncome;
        private ExtendedObservableCollection<GraphItemViewModel> _graphItems;
        private bool _isUserAdmin;
        private TransactionFilterViewModel _filterViewModel;

        #endregion
    }
}
