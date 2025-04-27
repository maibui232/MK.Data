namespace MK.Data
{
    using System;

    internal sealed class UInitConverter : Converter<uint>
    {
        protected override string ConvertToString(uint value, Type type)
        {
            return value.ToString();
        }

        protected override uint ConvertFromString(string text, Type type)
        {
            return uint.Parse(text);
        }
    }
}