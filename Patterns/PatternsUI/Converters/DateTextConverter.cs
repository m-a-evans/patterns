using System;
using System.Globalization;
using System.Windows.Data;

namespace PatternsUI.Converters
{
    internal class DateTextConverter : IValueConverter
    {
        private string _datePattern = "MM/dd/yyyy H:mm:ss zzz";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt && dt != DateTime.MinValue)
            {
                return dt.ToString(_datePattern);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string datetime && datetime != string.Empty)
            {
                return DateTime.Parse(datetime);
            }
            return DateTime.MinValue;
        }
    }
}
