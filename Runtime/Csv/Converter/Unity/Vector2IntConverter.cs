namespace MK.Data
{
    using System;
    using UnityEngine;

    internal class Vector2IntConverter : Converter<Vector2Int>
    {
        private readonly char separator;

        internal Vector2IntConverter(CsvSeparator separator)
        {
            this.separator = separator.Unity;
        }

        protected override string ConvertToString(Vector2Int value, Type type)
        {
            return $"{value.x}{this.separator}{value.y}";
        }

        protected override Vector2Int ConvertFromString(string text, Type type)
        {
            var values = text.Split(this.separator);

            return new Vector2Int(int.Parse(values[0]), int.Parse(values[1]));
        }
    }
}