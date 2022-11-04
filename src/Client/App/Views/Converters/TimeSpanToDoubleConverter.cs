using System.Globalization;

namespace Functionland.FxFiles.Client.App.Views.Converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is TimeSpan timespan)
            {
                return timespan.TotalSeconds;
            }

            return 1.0;
        }

        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is double doubleValue)
            {
                return TimeSpan.FromSeconds(doubleValue);
            }

            return TimeSpan.Zero;
        }
    }
}
