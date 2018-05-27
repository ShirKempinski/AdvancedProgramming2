using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Client_Side_of_Image_Service
{
    /// <summary>
    /// Converts the Log Status to a Brush color, implements IValueConverter
    /// </summary>
    public class StatusToColorConverter : IValueConverter
    {
        /// <summary>
        /// IValue Converter method
        /// </summary>
        /// <param name="value"> Object </param>
        /// <param name="targetType"> Type </param>
        /// <param name="parameter"> Object </param>
        /// <param name="culture"> CultureInfo </param>
        /// <returns>
        /// /// Converts "INFO" to LimeGreen
        /// Converts "WARNING" to Yellow
        /// Converts "FAIL" to IndianRed
        /// Converts the rest to Transparent
        /// </returns>
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

        
        /// <summary>
        /// IValue Converter method.
        /// Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
