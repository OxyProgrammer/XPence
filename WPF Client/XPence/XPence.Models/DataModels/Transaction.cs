using System;
using XPence.Infrastructure.BaseClasses;
using XPence.Shared;

namespace XPence.Models.DataModels
{
    /// <summary>
    /// A model class for a transaction.
    /// </summary>
    public class Transaction : ModelBase
    {
        /// <summary>
        /// Gets or sets the id of the transaction.
        /// This is the identifier property.
        /// </summary>
        public virtual long TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the date of the transaction
        /// </summary>
        public virtual DateTime TransactionDate { get; set; }

        /// <summary>
        /// Gets or sets the Amount of the transaction.
        /// </summary>
        public virtual double Amount { get; set; }

        /// <summary>
        /// Gets or sets the description of the transaction.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the Flow type of the transaction.
        /// </summary>
        public virtual TransactionFlowType FlowType { get; set; }

        /// <summary>
        /// Gets or sets the purpose of the transaction.
        /// </summary>
        public virtual TransactionPurposeType PurposeType { get; set; }

        /// <summary>
        /// Gets or sets the user that added the transaction.
        /// </summary>
        public virtual string LastModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the user that created the transaction. 
        /// </summary>
        public virtual string CreatedBy { get; set; }

        #region Overriden members

        /// <summary>
        /// Validates the property values against their names.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected override string GetErrorForProperty(string propertyName)
        {
            string error = null;
            switch (propertyName)
            {
                case "Amount":
                    if (Amount <= 0)
                        error = ErrorMessages.ERR_INCORRECT_AMOUNT;
                    break;
                case "Description":
                    if (null != Description && Description.Length > 255)
                        error = ErrorMessages.ERR_DESCRIPTION_TOO_LONG;
                    break;
            }

            return error;
        }

        #endregion

    }
}
