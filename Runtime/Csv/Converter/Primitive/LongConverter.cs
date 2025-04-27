namespace MK.Data
{
    using System;

    internal sealed class LongConverter : Converter<long>
    {
        protected override string ConvertToString(long value, Type type)
        {
            return value.ToString();
        }

        protected override long ConvertFromString(string text, Type type)
        {
            return long.Parse(text);
        }
    }
}