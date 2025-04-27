namespace MK.Data
{
    using System;

    internal sealed class ULongConverter : Converter<ulong>
    {
        protected override string ConvertToString(ulong value, Type type)
        {
            return value.ToString();
        }

        protected override ulong ConvertFromString(string text, Type type)
        {
            return ulong.Parse(text);
        }
    }
}