/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Windows.Input;
using XPence.Models.DataModels;
using XPence.Infrastructure.BaseClasses;
using System.Linq;
using XPence.Shared;

namespace XPence.ViewModels
{
    public class TransactionViewModel : ViewModelBase
    {
        #region Public Properies

        /// <summary>
        /// Gets the expense id.
        /// </summary>
        public long TransactionId
        {
            get { return Entity.TransactionId; }
        }

        /// <summary>
        /// Gets or sets the date of the transaction.
        /// </summary>
        public DateTime TransactionDate
        {
            get { return Entity.TransactionDate; }
            set
            {
                if (value.Date == Entity.TransactionDate)
                    return;
                Entity.TransactionDate = value.Date;
                OnPropertyChanged(GetPropertyName(() => TransactionDate));
            }
        }

        /// <summary>
        /// Gets or sets the description of an expense
        /// </summary>
        public string Description
        {
            get { return Entity.Description; }
            set
            {
                if (value == Entity.Description)
                    return;
                Entity.Description = value;
                OnPropertyChanged(GetPropertyName(() => Description));
            }
        }

        /// <summary>
        /// Gets or sets the Flow type of the transaction.
        /// </summary>
        public TransactionFlowType FlowType
        {
            get { return Entity.FlowType; }
            set
            {
                if (value == Entity.FlowType)
                    return;
                Entity.FlowType = value;
                OnPropertyChanged(GetPropertyName(() => FlowType));
            }
        }

        /// <summary>
        /// Gets or sets the purpose of the transaction.
        /// </summary>
        public TransactionPurposeType PurposeType
        {
            get { return Entity.PurposeType; }
            set
            {
                if (value == Entity.PurposeType)
                    return;
                Entity.PurposeType = value;
                OnPropertyChanged(GetPropertyName(() => PurposeType));
            }
        }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public string Amount
        {
            get { return _amount; }
            set
            {
                if (value == _amount)
                    return;
                _amount = value;
                double amount;
                if (double.TryParse(_amount, out amount))
                    Entity.Amount = amount;
                OnPropertyChanged(GetPropertyName(() => Amount));
            }
        }

        /// <summary>
        /// Gets or sets if the instance of <see cref="TransactionViewModel"/> is marked in the UI.
        /// </summary>
        public bool IsMarked
        {
            get { return _isMarked; }
            set
            {
                if (value == _isMarked)
                    return;
                _isMarked = value;
                OnPropertyChanged(GetPropertyName(() => IsMarked));
            }
        }

        /// <summary>
        /// Gets the username of the user who created the transaction.
        /// </summary>
        public string CreatedBy
        {
            get { return Entity.CreatedBy; }
        }

        /// <summary>
        /// Gets the username of the user who last modified the transaction.
        /// </summary>
        public string LastModifiedBy
        {
            get { return Entity.LastModifiedBy; }
        }

        /// <summary>
        /// Gets the wrapped up transaction model.
        /// </summary>
        public Transaction Entity { get; private set; }

        /// <summary>
        /// Returns if this instance of <see cref="TransactionViewModel"/> is valid for saving.
        /// </summary>
        public bool IsValid
        {
            get { return _propertyNames.All(p => GetErrorForProperty(p) == null); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method explicitly raises the property changed notification event for the <see cref="TransactionId"/>
        /// property for this instance.
        /// </summary>
        public void Refresh()
        {
            OnPropertyChanged(GetPropertyName(() => TransactionId));
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Intializes an instance of <see cref="TransactionViewModel"/>.
        /// </summary>
        /// <param name="entity"></param>
        public TransactionViewModel(Transaction entity)
        {
            if (null == entity)
                throw new ArgumentNullException("entity");
            Entity = entity;
            _amount = Convert.ToString(entity.Amount);
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Gets the error string for a property value against a property's name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string GetErrorForProperty(string propertyName)
        {
            string error = null;
            switch (propertyName)
            {
                case "Amount":
                    if (string.IsNullOrWhiteSpace(Amount))
                        error = ErrorMessages.ERR_MAND_AMOUNT_MESSAGE;
                    else
                    {
                        double amount;
                        if (!double.TryParse(Amount, out amount))
                            error = ErrorMessages.ERR_INCORRECT_AMOUNT;
                    }
                    break;
            }
            CommandManager.InvalidateRequerySuggested();
            return error ?? Entity[propertyName];
        }

        #endregion

        #region Member variables

        private string _amount;
        private bool _isMarked;
        private static readonly string[] _propertyNames = { "Amount", "TransactionDate", "Description", "PurposeType", "FlowType" };

        #endregion
    }
}
