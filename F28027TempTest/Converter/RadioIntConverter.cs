using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace F28027TempTest.Converter
{
    public class RadioIntConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((uint)value).Equals(uint.Parse((string)parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? int.Parse((string)parameter) : 0;
        }

        #endregion
    }
}
