using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BellaCodeAgenda.Converters
{
    public class ElapsedTimeDegreesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan? elapsedTime = null;
            TimeSpan? totalTime = null;

            if (values.Length > 0)
            {
                elapsedTime = values[0] as TimeSpan?;
            }

            if (values.Length > 1)
            {
                totalTime = values[1] as TimeSpan?;
            }

            if (elapsedTime.HasValue && elapsedTime.Value > TimeSpan.Zero && totalTime.HasValue && totalTime.Value > TimeSpan.Zero)
            {
                var percent = Math.Min(1.0, (elapsedTime.Value.TotalSeconds / totalTime.Value.TotalSeconds));
                return percent * 360.0;
            }

            return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
