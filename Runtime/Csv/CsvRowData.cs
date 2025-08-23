namespace MK.Data
{
    using System.Collections.Generic;

    public abstract class CsvRowData<TKey, TRecord> : Dictionary<TKey, TRecord>, ICsvData where TRecord : class, ICsvRecord
    {
    }
}