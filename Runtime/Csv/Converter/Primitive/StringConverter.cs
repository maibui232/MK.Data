namespace MK.Data
{
    using System;

    internal sealed class StringConverter : Converter<string>
    {
        protected override string ConvertToString(string value, Type type)
        {
            return value;
        }

        protected override string ConvertFromString(string text, Type type)
        {
            return text;
        }
    }
}