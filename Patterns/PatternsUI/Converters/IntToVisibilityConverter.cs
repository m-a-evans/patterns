using CommunityToolkit.Diagnostics;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PatternsUI.Converters
{
    /// <summary>
    /// Converter that translates a positive integer to Visible. Does not convert the other direction
    /// </summary>
    internal class IntToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ThrowHelper.ThrowNotSupportedException<object>();
        }
    }
}
