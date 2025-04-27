namespace MK.Data
{
    using System;

    internal sealed class IntConverter : Converter<int>
    {
        protected override string ConvertToString(int value, Type type)
        {
            return value.ToString();
        }

        protected override int ConvertFromString(string text, Type type)
        {
            return int.Parse(text);
        }
    }
}