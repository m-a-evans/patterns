using CommunityToolkit.Diagnostics;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PatternsUI.Converters
{
    class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }
            else
            {
                return ThrowHelper.ThrowNotSupportedException<bool>();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, CultureInfo.CurrentCulture);
        }
    }
}
