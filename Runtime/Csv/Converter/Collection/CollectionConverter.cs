namespace MK.Data
{
    using System;
    using System.Linq;

    internal abstract class CollectionConverter : IConverter
    {
        private readonly ConverterResolver converterResolver;
        private readonly char              separator;

        protected CollectionConverter(CsvSeparator separator, ConverterResolver converterResolver)
        {
            this.converterResolver = converterResolver;
            this.separator         = separator.Collection;
        }

        protected abstract Type TargetType { get; }

        Type IConverter.TargetType => this.TargetType;

        string IConverter.ConvertToString(object obj, Type type) => obj.ToString();

        object IConverter.ConvertFromString(string text, Type type) => this.ConvertFromString(text, type);

        protected virtual object ConvertFromString(string text, Type type)
        {
            var genericType = type.GetGenericArguments()[0];
            var converter   = this.converterResolver.GetConverter(type.GetGenericArguments()[0]);

            return text.Split(this.separator).Select(data => converter.ConvertFromString(data, genericType)).ToList();
        }
    }
}