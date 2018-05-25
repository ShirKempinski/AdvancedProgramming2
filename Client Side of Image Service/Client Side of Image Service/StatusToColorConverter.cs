using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Client_Side_of_Image_Service
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("Must convert to a brush");
            string status = (string)value;
            if (status.Contains("INFO")) { return Brushes.LimeGreen; }
            else if (status.Contains("WARNNING")) { return Brushes.LightYellow; }
            else if (status.Contains("FAIL")) { return Brushes.IndianRed; }
            else { return Brushes.Transparent; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
