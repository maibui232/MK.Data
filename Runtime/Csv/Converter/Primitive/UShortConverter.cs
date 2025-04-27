namespace MK.Data
{
    using System;

    internal sealed class UShortConverter : Converter<ushort>
    {
        protected override string ConvertToString(ushort value, Type type)
        {
            return value.ToString();
        }

        protected override ushort ConvertFromString(string text, Type type)
        {
            return ushort.Parse(text);
        }
    }
}