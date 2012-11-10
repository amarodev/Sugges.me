using Sugges.UI.Logic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Sugges.UI.Converters
{
    class ColorMoneyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return new SolidColorBrush() { Color = new Windows.UI.Color() { R = 166, G = 76, B = 0, A = 255 } }; //{StaticResource SuggesGroupTitleForegroundColor}
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
