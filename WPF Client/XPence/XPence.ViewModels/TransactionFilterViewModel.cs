/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Linq;
using System.Windows.Input;
using XPence.Infrastructure.BaseClasses;
using XPence.Infrastructure.CoreClasses;
using XPence.Infrastructure.MessagingService;
using XPence.Infrastructure.Utility;
using XPence.Models.Interfaces;
using XPence.Shared;

namespace XPence.ViewModels
{
    /// <summary>
    /// A ViewModel class to cater to the Filter view.
    /// </summary>
    public class TransactionFilterViewModel : FlyoutViewModelBase
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the from amount for filtering..
        /// </summary>
        public string FromAmount
        {
            get { return _fromAmount; }
            set
            {
                if (_fromAmount == value)
                    return;
                _fromAmount = value;
                OnPropertyChanged(GetPropertyName(() => FromAmount));
            }
        }

        /// <summary>
        /// Gets or sets the To Amount for filtering.
        /// </summary>
        public string ToAmount
        {
            get { return _toAmount; }
            set
            {
                if (_toAmount == value)
                    return;
                _toAmount = value;
                OnPropertyChanged(GetPropertyName(() => ToAmount));
            }
        }

        /// <summary>
        /// Gets or sets if the date range is applied in the filter.
        /// </summary>
        public bool IsDateRangeIncluded
        {
            get { return _isDateRangeIncluded; }
            set
            {
                if (_isDateRangeIncluded == value)
                    return;
                _isDateRangeIncluded = value;
                OnPropertyChanged(GetPropertyName(() => IsDateRangeIncluded));
                if (!_isDateRangeIncluded)
                    ResetDates();
            }
        }

        /// <summary>
        /// Gets or sets the user id to filter the transactions with.
        /// </summary>
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username == value)
                    return;
                _username = value;
                OnPropertyChanged(GetPropertyName(() => Username));
            }
        }


        /// <summary>
        /// Gets or sets the from date.
        /// </summary>
        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                if (_fromDate == value)
                    return;
                _fromDate = value;
                OnPropertyChanged(GetPropertyName(() => FromDate));
                OnPropertyChanged(GetPropertyName(() => ToDate));
            }
        }

        /// <summary>
        /// Gets or sets the to date.
        /// </summary>
        public DateTime ToDate
        {
            get { return _toDate; }
            set
            {
                if (_toDate == value)
                    return;
                _toDate = value;
                OnPropertyChanged(GetPropertyName(() => ToDate));
            }
        }

        /// <summary>
        /// Gets or sets all users list.
        /// </summary>
        public ExtendedObservableCollection<string> AllUsers
        {
            get { return _allUsers; }
            set
            {
                if (_allUsers != value)
                {
                    _allUsers = value;
                    OnPropertyChanged(GetPropertyName(() => AllUsers));
                }
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
                if (value != _isUserAdmin)
                {
                    if (_isUserAdmin == value)
                        return;
                    _isUserAdmin = value;
                    OnPropertyChanged(GetPropertyName(() => IsUserAdmin));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets or sets the command to get the trasnactions.
        /// </summary>
        public ICommand ApplyFilterCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of <see cref="TransactionFilterViewModel"/>.
        /// </summary>
        /// <param name="isUserAdmin"></param>
        /// <param name="userRepository"></param>
        /// <param name="messagingService"></param>
        public TransactionFilterViewModel(bool isUserAdmin, IUserRepository userRepository, IMessagingService messagingService)
        {
            if (userRepository == null)
                throw new ArgumentNullException("userRepository");
            if (messagingService == null)
                throw new ArgumentNullException("messagingService");
            _userRepository = userRepository;
            _messagingService = messagingService;
            IsUserAdmin = isUserAdmin;
            ResetDates();
            ApplyFilterCommand = new RelayCommand(ApplyFilter, CanApplyFilter);
            AllUsers = new ExtendedObservableCollection<string>(false);
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Returns error string for property value against property name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string GetErrorForProperty(string propertyName)
        {
            string error = null;
            switch (propertyName)
            {
                case "FromAmount":
                    if (!string.IsNullOrEmpty(FromAmount))
                    {
                        double amount = 0;
                        if (double.TryParse(FromAmount, out amount))
                        {
                            if (amount < 0)
                                error = ErrorMessages.ER_INVALID_AMOUNT;
                        }
                        else
                        {
                            error = ErrorMessages.ER_INVALID_AMOUNT;
                        }
                    }
                    break;
                case "ToAmount":
                    if (!string.IsNullOrEmpty(ToAmount))
                    {
                        double amount = 0;
                        if (double.TryParse(ToAmount, out amount))
                        {
                            if (amount < 0)
                                error = ErrorMessages.ER_INVALID_AMOUNT;
                        }
                        else
                        {
                            error = ErrorMessages.ER_INVALID_AMOUNT;
                        }
                    }
                    break;
                case "ToDate":
                    if (FromDate.Date > ToDate.Date)
                        error = ErrorMessages.ERR_FROM_TO_DATE;
                    break;
                case "Username":
                    if (AppData.LoggedInUser != null && AppData.LoggedInUser.Role != UserRole.Admin && !string.IsNullOrEmpty(Username))
                        error = ErrorMessages.ERR_CANT_SELECT_USER;
                    break;

            }
            return error;
        }

        /// <summary>
        /// Initializes the state amd data of the view model instance.
        /// </summary>
        protected override void OnInitialize()
        {
            try
            {
                var userList = _userRepository.GetAllUsers();
                AllUsers.Add(string.Empty);//For any user
                if (null != userList && userList.Any())
                {
                    var userNameList = userList.Select(u => u.Username);
                    AllUsers.AddRange(userNameList);
                    Username = string.Empty;//set the selected user to all.
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError("TransactionFilterViewModel", "OnInitialize", ex);
                _messagingService.ShowMessage(ErrorMessages.ERR_FAILED_TO_GET_USERS, DialogType.Error);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Resets the filter dates to the present day date.
        /// </summary>
        private void ResetDates()
        {
            FromDate = ToDate = DateTime.Now;
        }

        /// <summary>
        /// Executes the <see cref="ApplyFilterCommand"/>.
        /// </summary>
        private void ApplyFilter()
        {
            if (_filterApplied != null)
                _filterApplied(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets if <see cref="ApplyFilterCommand"/> can execute.
        /// </summary>
        /// <returns></returns>
        private bool CanApplyFilter()
        {
            return _propertyName.All(p => GetErrorForProperty(p) == null);
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when the user chooses to apply the filter after filling appropriate filter values.
        /// </summary>
        public event EventHandler FilterApplied
        {
            add { _filterApplied += value; }
            remove { _filterApplied -= value; }
        }

        #endregion

        #region Member Variables

        private event EventHandler _filterApplied;
        private bool _isDateRangeIncluded;
        private DateTime _fromDate;
        private DateTime _toDate;
        private string _fromAmount;
        private string _toAmount;
        private string _username;
        private bool _isUserAdmin;
        private readonly IUserRepository _userRepository;
        private readonly IMessagingService _messagingService;
        private ExtendedObservableCollection<string> _allUsers;

        #endregion

        private static string[] _propertyName = new string[]{
                                                      "ToDate",
                                                      "FromAmount",
                                                      "ToAmount",
                                                      "Username"
                                                  };
    }
}
