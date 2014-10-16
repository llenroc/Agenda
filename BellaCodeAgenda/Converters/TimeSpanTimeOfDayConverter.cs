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

            int minutes = 0;
            if (!string.IsNullOrEmpty(minutesText))
            {
                minutes = int.Parse(minutesText);
            }

            if (hours > 24)
            {
                return DependencyProperty.UnsetValue;
            }

            if (minutes > 60)
            {
                return DependencyProperty.UnsetValue;
            }


            // If there is no hint and the user entered a number that isn't clearly AM/PM on a 24-hour clock
            if (isPmHint == null && hours > 0 && hours <= 12)
            {
                var amHour = hours%12;
                var pmHour = hours%12 + 12;

                var nowHour = DateTime.Now.Hour;
                var isAm = nowHour < 12;

                if (isAm && nowHour > amHour)
                {
                    isPmHint = true;
                }
                else if (!isAm && nowHour <= pmHour)
                {
                    isPmHint = true;
                }
                else if (hours == 12)
                {
                    isPmHint = false;
                }


#if false // brute force conditional logic algorithm
                var nowHour = DateTime.Now.Hour;

                // If it is 12 AM, then I never adjust the time to PM.
                if (nowHour != 0)
                {
                    // If it is AM
                    if (nowHour < 12)
                    {
                        // I consider "12" and any hours that are before the current hour to mean PM                                                
                        if (hours == 12 || nowHour - hours > 0)
                        {
                            isPmHint = true;
                        }
                    }

                    // If it is PM
                    if (nowHour >= 12)
                    {
                        var adjustedNowHour = nowHour - 12;

                        // I treat "12" as a special case because it is larger than other 12-hour clock PM numbers
                        if (hours == 12)
                        {
                            isPmHint = (adjustedNowHour == 0);
                        }
                        // I consider any hours that are on or after the current hour to mean PM
                        else if (hours - adjustedNowHour >= 0)
                        {
                            isPmHint = true;
                        }
                    }
                }
#endif
            }

            // If there is a hint that this is a PM time, I add 12 hours.
            if (isPmHint == true && hours < 12)
            {
                hours += 12;
            }
            // If there is a hint that this is AM, I convert 12 to 0.
            else if (isPmHint == false && hours == 12)
            {
                hours = 0;
            }

            // Now Hour: 0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23
            // PM:       

            

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
