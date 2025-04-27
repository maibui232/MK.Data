namespace MK.Data
{
    using System;
    using UnityEngine;

    internal class Vector2Converter : Converter<Vector2>
    {
        private readonly char separator;

        internal Vector2Converter(CsvSeparator csvSeparator)
        {
            this.separator = csvSeparator.Unity;
        }

        protected override string ConvertToString(Vector2 value, Type type)
        {
            return $"{value.x}{this.separator}{value.y}";
        }

        protected override Vector2 ConvertFromString(string text, Type type)
        {
            var values = text.Split(this.separator);

            return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        }
    }
}