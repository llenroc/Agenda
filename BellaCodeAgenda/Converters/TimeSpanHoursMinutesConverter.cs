using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BellaCodeAgenda.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanHoursMinutesConverter : IValueConverter
    {
        public const string DefaultZeroText = "0h 00m";

        public const string DefaultNullText = "0h 00m";

        public TimeSpanHoursMinutesConverter()
        {
            this.NullValueText = DefaultNullText;
            this.ZeroValueText = DefaultZeroText;
        }

        public string NullValueText { get; set; }

        public string ZeroValueText { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var timeSpan = value as TimeSpan?;

            if (!timeSpan.HasValue)
            {
                return this.NullValueText;
            }

            if (timeSpan == TimeSpan.Zero)
            {
                return this.ZeroValueText;
            }

            // I convert negative to positive values
            if (timeSpan < TimeSpan.Zero)
            {
                timeSpan = new TimeSpan(Math.Abs(timeSpan.Value.Ticks));
            }

            var integer = Math.Truncate(timeSpan.Value.TotalHours);
            var fraction = timeSpan.Value.TotalHours - integer;

            int hours = (int)integer;
            int minutes = (int)Math.Round(60.0 * fraction, 0);

            return string.Format("{0}h {1:d2}m", hours, minutes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = (string)value;

            if (!string.IsNullOrEmpty(text))
            {
                int? hours = null;
                int? minutes = null;
                StringBuilder pending = new StringBuilder();
                foreach (char c in text)
                {
                    if (char.IsDigit(c))
                    {
                        pending.Append(c);
                    }
                    else if (char.IsWhiteSpace(c) && pending.Length == 0)
                    {
                        continue;
                    }                   
                    else
                    {
                        // h/H indicates hours
                        if (!hours.HasValue && (c == 'h' || c == 'H'))
                        {
                            hours = (pending.Length > 0) ? int.Parse(pending.ToString()) : 0;
                            pending.Clear();
                        }
                        // m/M indicates minutes
                        else if (!minutes.HasValue && (c == 'm' || c == 'M'))
                        {
                            minutes = (pending.Length > 0) ? int.Parse(pending.ToString()) : 0;
                            pending.Clear();
                        }
                        // otherwise the first number encountered is hours and the second number minutes.
                        else if (pending.Length > 0)
                        {
                            if (!hours.HasValue)
                            {
                                hours = int.Parse(pending.ToString());
                                pending.Clear();
                            }
                            else if (!minutes.HasValue)
                            {
                                minutes = int.Parse(pending.ToString());
                                pending.Clear();
                            }
                        }
                    }
                }

                // I handle any remaining number here
                if (pending.Length > 0)
                {
                    if (!hours.HasValue)
                    {
                        hours = int.Parse(pending.ToString());
                    }
                    else if (!minutes.HasValue)
                    {
                        minutes = int.Parse(pending.ToString());
                    }
                }

                // If there was text, but I couldn't find hours or minutes, return UnSetValue
                if (!hours.HasValue && !minutes.HasValue)
                {
                    return DependencyProperty.UnsetValue;
                }

                // If I didn't find hours or minutes then I default to zero
                hours = hours ?? 0;
                minutes = minutes ?? 0;

                return TimeSpan.FromHours(hours.Value) + TimeSpan.FromMinutes(minutes.Value);
            }

            // I return zero for empty text
            return TimeSpan.Zero;
        }
    }
}
