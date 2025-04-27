namespace MK.Data
{
    using System;

    public interface IConverter
    {
        internal Type   TargetType { get; }
        internal string ConvertToString(object   obj,  Type type);
        internal object ConvertFromString(string text, Type type);
    }

    public abstract class Converter<T> : IConverter
    {
        Type IConverter.TargetType => typeof(T);

        string IConverter.ConvertToString(object obj, Type type)
        {
            return this.ConvertToString((T)obj, type);
        }

        object IConverter.ConvertFromString(string text, Type type)
        {
            return this.ConvertFromString(text, type);
        }

        protected abstract string ConvertToString(T value, Type type);

        protected abstract T ConvertFromString(string text, Type type);
    }
}