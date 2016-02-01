
using System;

namespace XPence.Models.DataModels
{
    /// <summary>
    /// A wrapper class to carry all the filter criteria fields.
    /// </summary>
    public class TransactionFilter
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the to amount filter.
        /// </summary>
        public double? ToAmount { get; set; }

        /// <summary>
        /// Gets or sets FromDate
        /// </summary>
        public DateTime? FromDate
        {
            get { return _fromDate; }
            set
            {
                if (value.HasValue)
                {
                    _fromDate = value.Value.Date;
                }
            }
        }

        /// <summary>
        /// Gets or sets ToDate
        /// </summary>
        public DateTime? ToDate
        {
            get { return _toDate; }
            set
            {
                if (value.HasValue)
                {
                    _toDate = value.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
        }

        /// <summary>
        /// Gets or sets the from amount filter.
        /// </summary>
        public double? FromAmount { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string Username { get; set; }

        #endregion

        #region Member Variables

        private DateTime? _fromDate;
        private DateTime? _toDate;

        #endregion
    }
}
