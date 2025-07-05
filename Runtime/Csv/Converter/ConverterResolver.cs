namespace MK.Data
{
    using System;
    using System.Collections.Generic;
    using MK.Kernel;

    internal sealed class ConverterResolver
    {
        private readonly IResolver                    resolver;
        private readonly Dictionary<Type, IConverter> typeToConverter = new();

        public ConverterResolver(IResolver resolver)
        {
            this.resolver = resolver;
        }

        public void AddConverter<T>() where T : IConverter
        {
            var type = typeof(T);
            if (this.typeToConverter.ContainsKey(type))
            {
                throw new ArgumentException($"Converter '{typeof(T)}' already registered.");
            }

            this.typeToConverter.Add(type, this.resolver.Instantiate<T>());
        }

        public IConverter GetConverter<T>()
        {
            return this.GetConverter(typeof(T));
        }

        public IConverter GetConverter(Type type)
        {
            if (this.typeToConverter.TryGetValue(type, out var converter))
            {
                return converter;
            }

            throw new ArgumentException($"Converter '{type}' is not registered.");
        }
    }
}