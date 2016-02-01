/***************************************************************************************************
 * PROJECT : XPence
 * PROJECT DESCRIPTION : A metro style, smart client expense tracking software.
 * AUTHOR : Siddhartha S
 * DISCLAIMER : This code is licensed under CPOL. You are free to use this in your project.
 * The author takes no liabilities for any damage caused because of this code. Use at your own risk.
****************************************************************************************************/

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro;
using XPence.Infrastructure.Utility;

namespace XPence.Converters
{
    /// <summary>
    /// An implementation of <see cref="IValueConverter"/> contract that converts a string to a <see cref="Brush"/> object.
    /// </summary>
    public class StringToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts the name of a color to a <see cref="System.Windows.Media.Brush"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var purpose = System.Convert.ToString(parameter);
            switch (purpose)
            {
                case AppConstants.CONVERT_ACCENT:
                    var aceentColorName = System.Convert.ToString(value);
                    var accent = ThemeManager.DefaultAccents.FirstOrDefault(a => string.CompareOrdinal(a.Name, aceentColorName) == 0);
                    if (null != accent)
                        return accent.Resources["AccentColorBrush"] as Brush;
                    break;
                case AppConstants.CONVERT_BASE:
                    var baseColorName = System.Convert.ToString(value);
                    var converter = new BrushConverter();
                    if (string.CompareOrdinal(baseColorName, AppConstants.LIGHT_BASE) == 0)
                    {
                        var brush = (Brush)converter.ConvertFromString("#FFFFFFFF");
                        return brush;
                    }
                    if (string.CompareOrdinal(baseColorName, AppConstants.DARK_BASE) == 0)
                    {
                        var brush = (Brush)converter.ConvertFromString("#FF000000");
                        return brush;
                    }

                    break;
            }
            return null;
        }

        /// <summary>
        /// Not implemented!
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
