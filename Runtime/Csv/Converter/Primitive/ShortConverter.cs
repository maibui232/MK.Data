namespace MK.Data
{
    using System;

    internal sealed class ShortConverter : Converter<short>
    {
        protected override string ConvertToString(short value, Type type)
        {
            return value.ToString();
        }

        protected override short ConvertFromString(string text, Type type)
        {
            return short.Parse(text);
        }
    }
}