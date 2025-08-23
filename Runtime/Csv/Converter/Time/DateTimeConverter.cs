namespace MK.Data
{
    using System;
    using System.Globalization;

    internal sealed class DateTimeConverter : Converter<DateTime>
    {
        protected override string ConvertToString(DateTime value, Type type)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        protected override DateTime ConvertFromString(string text, Type type)
        {
            return DateTime.Parse(text, CultureInfo.InvariantCulture);
        }
    }
}