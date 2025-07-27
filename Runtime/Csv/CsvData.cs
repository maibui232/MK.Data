namespace MK.Data
{
    using System.Collections.Generic;

    public abstract class CsvData<T> : List<T> where T : class, ICsvData
    {
    }

    public abstract class CsvData<TKey, TValue> : Dictionary<TKey, TValue> where TValue : class, ICsvData
    {
    }
}