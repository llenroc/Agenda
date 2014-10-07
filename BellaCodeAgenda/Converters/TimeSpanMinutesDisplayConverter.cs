using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BellaCodeAgenda.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanMinutesDisplayConverter : IValueConverter
    {
        private const string DefaultMinuteSuffix = "minute";

        private const string DefaultMinutesSuffix = "minutes";

        public TimeSpanMinutesDisplayConverter()
        {
            this.MinuteSuffix = DefaultMinuteSuffix;
            this.MinutesSuffix = DefaultMinutesSuffix;
        }

        public string MinuteSuffix { get; set; }

        public string MinutesSuffix { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var timeSpan = value as TimeSpan?;
            if (timeSpan.HasValue)
            {
                var minutes = (int)Math.Floor(timeSpan.Value.TotalMinutes);
                if (timeSpan.Value.TotalMinutes > 0 && timeSpan.Value.TotalMinutes < 1)
                {
                    return "<1 " + this.MinuteSuffix;
                }
                return string.Format("{0} {1}", minutes, minutes == 1 ? this.MinuteSuffix : this.MinutesSuffix);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("TimeSpanMinutesDisplayConverter cannot convert a string back to a TimeSpan.");
        }
    }
}
