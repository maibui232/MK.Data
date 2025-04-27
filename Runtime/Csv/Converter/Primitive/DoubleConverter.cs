namespace MK.Data
{
    using System;
    using System.Globalization;

    internal sealed class DoubleConverter : Converter<double>
    {
        protected override string ConvertToString(double value, Type type)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        protected override double ConvertFromString(string text, Type type)
        {
            return double.Parse(text);
        }
    }
}