namespace MK.Data
{
    using System;
    using System.Globalization;

    internal sealed class FloatConverter : Converter<float>
    {
        protected override string ConvertToString(float value, Type type)
        {
            return float.IsNaN(value) ? "NaN" : value.ToString(CultureInfo.InvariantCulture);
        }

        protected override float ConvertFromString(string text, Type type)
        {
            return float.Parse(text, CultureInfo.InvariantCulture);
        }
    }
}