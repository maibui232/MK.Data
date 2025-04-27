namespace MK.Data
{
    using System;
    using UnityEngine;

    internal class Vector3Converter : Converter<Vector3>
    {
        private readonly char separator;

        public Vector3Converter(CsvSeparator separator)
        {
            this.separator = separator.Unity;
        }

        protected override string ConvertToString(Vector3 value, Type type)
        {
            return $"{value.x}{this.separator}{value.y}{this.separator}{value.z}";
        }

        protected override Vector3 ConvertFromString(string text, Type type)
        {
            var values = text.Split(this.separator);

            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
    }
}