using System.Globalization;

namespace Functionland.FxFiles.Client.App.Views.Converters
{
    public class TimeSpanToStringConverter : ValueConverter<TimeSpan?, string>
    {
        public override string Convert(TimeSpan? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not null)
            {
                string formatted = string.Format("{0:00:}{1:00:}{2:00}", value.Value.TotalHours == 0 ? String.Empty : "", value.Value.Minutes, value.Value.Seconds);
                return formatted;
            }

            return string.Empty;
        }

        public override TimeSpan? ConvertBack(string value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
