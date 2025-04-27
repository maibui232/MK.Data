namespace MK.Data
{
    using System;

    internal sealed class CharConverter : Converter<char>
    {
        protected override string ConvertToString(char value, Type type)
        {
            return value.ToString();
        }

        protected override char ConvertFromString(string text, Type type)
        {
            return char.Parse(text);
        }
    }
}