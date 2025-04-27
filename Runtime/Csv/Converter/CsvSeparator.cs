namespace MK.Data
{
    internal class CsvSeparator
    {
        internal char Unity        { get; }
        internal char Collection   { get; }
        internal char KeyValuePair { get; }

        public CsvSeparator(char unity = '|', char collection = ';', char keyValuePair = ':')
        {
            this.Unity        = unity;
            this.Collection   = collection;
            this.KeyValuePair = keyValuePair;
        }
    }
}