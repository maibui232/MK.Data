namespace MK.Data
{
    using System;
    using UnityEngine;

    internal class Vector3IntConverter : Converter<Vector3Int>
    {
        private readonly char separator;

        internal Vector3IntConverter(CsvSeparator separator)
        {
            this.separator = separator.Unity;
        }

        protected override string ConvertToString(Vector3Int value, Type type)
        {
            return $"{value.x}{this.separator}{value.y}{this.separator}{value.z}";
        }

        protected override Vector3Int ConvertFromString(string text, Type type)
        {
            var values = text.Split(this.separator);

            return new Vector3Int(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
        }
    }
}