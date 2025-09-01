namespace MK.Data
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CsvIgnoreAttribute : Attribute
    {
    }
}