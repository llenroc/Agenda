using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BellaCodeAgenda.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class RemainingTimeSpanDisplayConverter : IValueConverter
    {
        private const string DefaultOneHourSuffix = "hour";
        private const string DefaultHoursSuffix = "hours";

        private const string DefaultOneMinuteSuffix = "minute";
        private const string DefaultMinutesSuffix = "minutes";

        public RemainingTimeSpanDisplayConverter()
        {
            this.OneHourSuffix = DefaultOneHourSuffix;
            this.HoursSuffix = DefaultHoursSuffix;
            this.OneMinuteSuffix = DefaultOneMinuteSuffix;
            this.MinutesSuffix = DefaultMinutesSuffix;
        }

        public string OneHourSuffix { get; set; }

        public string HoursSuffix { get; set; }

        public string OneMinuteSuffix { get; set; }

        public string MinutesSuffix { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = value as TimeSpan?;
            if (input.HasValue)
            {
                var timeSpan = input.Value;

                if (timeSpan <= TimeSpan.Zero)
                {
                    return "0 " + this.MinutesSuffix;
                }

                int hours = (int)Math.Truncate(timeSpan.TotalHours);
                int minutes = timeSpan.Minutes;

                var text = string.Format("{0} {1}", minutes, minutes == 1 ? this.OneMinuteSuffix : this.MinutesSuffix);

                if (hours > 0)
                {
                    text = string.Format("{0} {1} {2}", hours, hours == 1 ? this.OneHourSuffix : this.HoursSuffix, text);
                }

                return text;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("TimeSpanMinutesDisplayConverter cannot convert a string back to a TimeSpan.");
        }
    }
}
