using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenAttractor.Converters
{
    public class Multiplier
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dblValue = 1.0;
            if (value is double)
                dblValue = (double)value;
            else if (!(value is string) || !double.TryParse((string)value, out dblValue))
                return null;

            var dblParam = 1.0;
            if (parameter is double)
                dblParam = (double)parameter;
            else if (!(parameter is string) || !double.TryParse((string)parameter, out dblParam))
                return null;

            return dblValue * dblParam;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
