namespace MK.Data
{
    using System;
    using UnityEngine;

    internal class ColorConverter : Converter<Color>
    {
        private readonly char separator;

        internal ColorConverter(CsvSeparator separator)
        {
            this.separator = separator.Unity;
        }

        protected override string ConvertToString(Color value, Type type)
        {
            return $"{value.r}{this.separator}{value.g}{this.separator}{value.b}{this.separator}{value.a}";
        }

        protected override Color ConvertFromString(string text, Type type)
        {
            var values = text.Split(this.separator);

            return new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
    }
}