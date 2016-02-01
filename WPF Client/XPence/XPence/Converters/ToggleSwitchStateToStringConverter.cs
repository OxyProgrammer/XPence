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
using XPence.Shared;

namespace XPence.Converters
{
    /// <summary>
    /// An implementation of <see cref="IValueConverter"/> to convert the checked state of <see cref="MahApps.Metro.Controls.ToggleSwitch"/> to <see cref="String"/>.
    /// </summary>
    public class ToggleSwitchStateToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the checked state of the <see cref="MahApps.Metro.Controls.ToggleSwitch"/> state to appropriate text.
        /// </summary>
        /// <param name="value">The checked state of the toggle switch.</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>"On" if the <see cref="MahApps.Metro.Controls.ToggleSwitch"/> is in checked state and returns "Off" otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool returnValue;
            bool.TryParse(System.Convert.ToString(value), out returnValue);
            return returnValue ? UIText.ON_LABEL_TEXT : UIText.OFF_LABEL_TEXT;
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
