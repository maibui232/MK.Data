namespace MK.Data
{
    using System;

    internal sealed class ArrayConverter : CollectionConverter
    {
        public ArrayConverter(CsvSeparator separator, ConverterResolver converterResolver) : base(separator, converterResolver)
        {
        }

        protected override Type TargetType => typeof(Array);

        protected override object ConvertFromString(string text, Type type)
        {
            return base.ConvertFromString(text, type) as object[];
        }
    }
}