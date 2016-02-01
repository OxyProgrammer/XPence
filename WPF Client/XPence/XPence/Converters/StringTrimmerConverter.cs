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
    /// 
    /// </summary>
    public class StringTrimmerConverter:IValueConverter
    {
        /// <summary>
        /// Gets o sets the max numbers of characters allowed including the three ellispis dots.
        /// This instance of <see cref="StringTrimmerConverter"/> will not work if the value is set less than 3.
        /// </summary>
        public int MaxLengthAllowed { get; set; }

        /// <summary>
        /// Curtails a string value and provides a substring value suffixxed with ellipsis.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var arg = System.Convert.ToString(value);
            if(MaxLengthAllowed>3 && arg.Length>MaxLengthAllowed)
            {
              return string.Concat(arg.Substring(0,MaxLengthAllowed - 3), "...");
            }
            return arg;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
