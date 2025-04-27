namespace MK.Data
{
    using System;
    using System.Collections.Generic;

    internal sealed class HashsetConverter : CollectionConverter
    {
        public HashsetConverter(CsvSeparator separator, ConverterResolver converterResolver) : base(separator, converterResolver)
        {
        }

        protected override Type TargetType => typeof(HashSet<>);

        protected override object ConvertFromString(string text, Type type)
        {
            return base.ConvertFromString(text, type) as HashSet<object>;
        }
    }
}