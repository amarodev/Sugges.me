using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sugges.UI.Converters
{
    /// <summary>
    /// Value converter that translates true to false and vice versa.
    /// </summary>
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }
 
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}