namespace MK.Data
{
    using System;
    using UnityEngine;

    internal class QuaternionConverter : Converter<Quaternion>
    {
        private readonly char separator;

        public QuaternionConverter(CsvSeparator separator)
        {
            this.separator = separator.Unity;
        }

        protected override string ConvertToString(Quaternion value, Type type)
        {
            return $"{value.x}{this.separator}{value.y}{this.separator}{value.z}{this.separator}{value.w}";
        }

        protected override Quaternion ConvertFromString(string text, Type type)
        {
            var values = text.Split(this.separator);

            return new Quaternion(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
    }
}