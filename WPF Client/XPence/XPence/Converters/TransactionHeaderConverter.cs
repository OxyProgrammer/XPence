/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Globalization;
using System.Windows.Data;

namespace XPence.Converters
{
    /// <summary>
    /// An implementation of <see cref="IValueConverter"/> to provide the header text for a transaction.
    /// </summary>
    public class TransactionHeaderConverter : IMultiValueConverter
    {
        /// <summary>
        /// returns a header text for transaction.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //Expected parameters are Transaction id, transaction date and amount
            if (values.Length != 3)
                return string.Empty;

            long transId;
            DateTime transDate;
            double amount;

            //If any of them is an invalid value we not gonna do anything.
            if (!long.TryParse(System.Convert.ToString(values[0]), out transId))
                return string.Empty;
            if (!DateTime.TryParse(System.Convert.ToString(values[1]), out transDate))
                return string.Empty;
            if (!double.TryParse(System.Convert.ToString(values[2]), out amount))
                return string.Empty;
            if (transId == 0)
                return "New transaction";//Should get this from a resource file
            return string.Format("{0:dd-MM-yyyy} - ({1})", transDate, amount);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
