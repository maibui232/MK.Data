namespace MK.Data
{
    using System;

    internal sealed class DateTimeOffsetConverter : Converter<DateTimeOffset>
    {
        protected override string ConvertToString(DateTimeOffset value, Type type)
        {
            return value.ToString();
        }

        protected override DateTimeOffset ConvertFromString(string text, Type type)
        {
            return DateTimeOffset.Parse(text);
        }
    }
}