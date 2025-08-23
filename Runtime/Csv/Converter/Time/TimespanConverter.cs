namespace MK.Data
{
    using System;

    internal sealed class TimespanConverter : Converter<TimeSpan>
    {
        protected override string ConvertToString(TimeSpan value, Type type)
        {
            return value.ToString();
        }

        protected override TimeSpan ConvertFromString(string text, Type type)
        {
            return TimeSpan.Parse(text);
        }
    }
}