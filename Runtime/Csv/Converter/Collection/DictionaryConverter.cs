namespace MK.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class DictionaryConverter : CollectionConverter
    {
        public DictionaryConverter(CsvSeparator separator, ConverterResolver converterResolver) : base(separator, converterResolver)
        {
        }

        protected override Type TargetType => typeof(Dictionary<,>);

        protected override object ConvertFromString(string text, Type type)
        {
            return base.ConvertFromString(text, type) as Dictionary<object, object>;
        }
    }
}