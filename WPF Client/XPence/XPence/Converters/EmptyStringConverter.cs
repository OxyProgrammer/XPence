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
    /// A converter to convert empty or null string with the supplied parameter value.
    /// </summary>
    public class EmptyStringConverter :  IValueConverter
    {
        /// <summary>
        /// Returns the <see cref="value"/> if it is not empty or null.
        /// Returns the <see cref="parameter"/> otherwise.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType,object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty(System.Convert.ToString(value)) ? parameter : value;
        }

        /// <summary>
        /// No Implemented!
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType,object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
