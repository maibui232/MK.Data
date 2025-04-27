namespace MK.Data
{
    using System;
    using System.Collections.Generic;

    internal sealed class KeyValuePairConverter : IConverter
    {
        private readonly ConverterResolver converterResolver;
        private readonly char              separator;

        public KeyValuePairConverter(CsvSeparator csvSeparator, ConverterResolver converterResolver)
        {
            this.converterResolver = converterResolver;
            this.separator         = csvSeparator.KeyValuePair;
        }

        Type IConverter.TargetType => typeof(KeyValuePair<,>);

        string IConverter.ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }

        object IConverter.ConvertFromString(string text, Type type)
        {
            var splits    = text.Split(this.separator);
            var keyType   = ((IConverter)this).TargetType.GetGenericArguments()[0];
            var valueType = ((IConverter)this).TargetType.GetGenericArguments()[1];
            var key       = this.converterResolver.GetConverter(keyType).ConvertFromString(splits[0], keyType);
            var value     = this.converterResolver.GetConverter(valueType).ConvertFromString(splits[1], valueType);

            return Activator.CreateInstance(typeof(KeyValuePair<,>), key, value);
        }
    }
}