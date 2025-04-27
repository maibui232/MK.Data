namespace MK.Data
{
    using System;
    using System.Collections.Generic;

    internal sealed class ListConverter : CollectionConverter
    {
        public ListConverter(CsvSeparator separator, ConverterResolver converterResolver) : base(separator, converterResolver)
        {
        }

        protected override Type TargetType => typeof(List<>);

        protected override object ConvertFromString(string text, Type type)
        {
            return base.ConvertFromString(text, type) as List<object>;
        }
    }
}