namespace MK.Data
{
    using System;
    using System.Collections.Generic;
    using MK.Extensions;
    using MK.Kernel;

    internal sealed class ConverterResolver : IInitializable
    {
        private readonly IResolver                    resolver;
        private readonly Dictionary<Type, IConverter> typeToConverter = new();

        public ConverterResolver(IResolver resolver)
        {
            this.resolver = resolver;
        }

        void IInitializable.Initialize()
        {
            typeof(IConverter).GetDerivedTypes()
               .ForEach(type =>
                        {
                            var converter = (IConverter)this.resolver.Instantiate(type);
                            this.typeToConverter.Add(converter.TargetType, converter);
                        });
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