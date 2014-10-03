using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BellaCodeAgenda.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanMinutesConverter : IValueConverter
    {        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var timeSpan = value as TimeSpan?;
            if (timeSpan.HasValue)
            {
                var minutes = (int)Math.Floor(timeSpan.Value.TotalMinutes);
                return minutes.ToString();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var text = value as string;

            if (text != null)
            {
                int minutes;
                if (int.TryParse(text, out minutes))
                {
                    return TimeSpan.FromMinutes(minutes);
                }
            }

            return TimeSpan.Zero;
        }
    }
}
