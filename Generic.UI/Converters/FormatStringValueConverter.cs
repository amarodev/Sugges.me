using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Generic.UI.Converters
{
    public class FormatStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null)
                return string.Empty;

            return !string.IsNullOrEmpty(parameter.ToString()) ? string.Format(parameter.ToString(), value).ToUpper() : value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException(); 
        }
    }
}
