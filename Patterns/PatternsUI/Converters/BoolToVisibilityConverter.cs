﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PatternsUI.Converters
{
    /// <summary>
    /// Converter that translates true => Visible and vice versa
    /// </summary>
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
            {
                return vis == Visibility.Visible;
            }
            return false;
        }
    }
}
