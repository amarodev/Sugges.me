using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sugges.UI.Converters
{
    /// <summary>
    /// Value converter that translates true to false and vice versa.
    /// </summary>
    public sealed class ValueToMoneyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? "None" : value.ToString() + " USD"; //TODO Apply selected money
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}