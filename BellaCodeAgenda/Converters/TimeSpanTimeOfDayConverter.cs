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
    public class TimeSpanTimeOfDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var timeSpan = value as TimeSpan?;
            if (timeSpan.HasValue)
            {
                // I convert negative to positive values
                if (timeSpan < TimeSpan.Zero)
                {
                    timeSpan = new TimeSpan(Math.Abs(timeSpan.Value.Ticks));
                }

                int hours = timeSpan.Value.Hours % 12;
                int minutes = timeSpan.Value.Minutes;
                bool isAm = timeSpan.Value.Hours < 12;

                if (hours == 0)
                {
                    hours = 12;
                }

                return string.Format("{0}:{1:d2} {2}", hours, minutes, isAm ? "AM" : "PM");
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var text = value as string;

            if (string.IsNullOrEmpty(text))
            {
                return TimeSpan.Zero;
            }

            text = text.Trim();
            text = text.ToUpperInvariant();

            var hoursText = (string)null;
            var minutesText = (string)null;
            var isPmHint = (bool?)null;
            var numberText = new StringBuilder();

            foreach (char c in text)
            {
                // I build up the digits
                if (char.IsDigit(c))
                {
                    numberText.Append(c);
                }
                else
                {
                    // If we see an A or a P, the first one hints at AM/PM
                    if (char.IsLetter(c) && !isPmHint.HasValue)
                    {
                        if (c == 'A')
                        {
                            isPmHint = false;
                        }
                        else if (c == 'P')
                        {
                            isPmHint = true;
                        }
                    }

                    // I process the number text whenever a non-digit occurs                
                    SetHoursOrMinutesText(numberText, ref hoursText, ref minutesText);
                }
            }

            // I process the number text at the end of the string.
            SetHoursOrMinutesText(numberText, ref hoursText, ref minutesText);

            if (string.IsNullOrEmpty(hoursText))
            {
                return DependencyProperty.UnsetValue;
            }

            int hours = int.Parse(hoursText);
            
            // If there is a hint that this is a PM time, I add 12 hours.
            if (isPmHint == true && hours < 12)
            {
                hours += 12;
            }
            // If there is a hint that this is AM, I conver 12 to 0.
            else if (isPmHint == false && hours == 12)
            {
                hours = 0;
            }
            // If there is no hint, then I assume working hours.
            // Numbers between 7-11 are AM, and 12-6 are PM
            else if (isPmHint == null)
            {
                if (hours >= 1 && hours <= 6)
                {
                    hours += 12;
                }
            }

            int minutes = 0;
            if (!string.IsNullOrEmpty(minutesText))
            {
                minutes = int.Parse(minutesText);
            }

            var timeSpan = TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes);

            // I constrain the the resulting 24 hour period.
            return TimeSpan.FromHours(timeSpan.Hours) + TimeSpan.FromMinutes(timeSpan.Minutes);
        }

        // The first set of digits are hours, the second set of digits is minutes.
        private static void SetHoursOrMinutesText(StringBuilder numberText, ref string hoursText, ref string minutesText)
        {
            if (numberText.Length > 0)
            {
                if (hoursText == null)
                {
                    hoursText = numberText.ToString();
                }
                else if (minutesText == null)
                {
                    minutesText = numberText.ToString();
                }

                numberText.Clear();
            }
        }
    }
}
