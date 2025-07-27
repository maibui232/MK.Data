namespace MK.Data
{
    using System;
    using Cysharp.Threading.Tasks;

    internal sealed class CsvStreamer
    {
        private readonly ConverterResolver resolver;

        public CsvStreamer(ConverterResolver resolver)
        {
            this.resolver = resolver;
        }

        public UniTask<IData> ReadAsync(Type type, string csvData)
        {
            return UniTask.FromResult<IData>(null);
        }

        public UniTask WriteAsync(IData data)
        {
            return UniTask.CompletedTask;
        }
    }
}