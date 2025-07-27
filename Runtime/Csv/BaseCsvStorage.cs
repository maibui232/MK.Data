namespace MK.Data
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using MK.Log;

    internal abstract class BaseCsvStorage<TInterface> : BaseStorage<TInterface, ICsvData> where TInterface : IStorage
    {
        private readonly CsvStreamer csvStreamer;

        protected BaseCsvStorage(ILoggerManager loggerManager, CsvStreamer csvStreamer) : base(loggerManager)
        {
            this.csvStreamer = csvStreamer;
        }

        protected override async UniTask<IData> DeserializeAsync(string key, Type type, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return await this.csvStreamer.ReadAsync(type, await this.LoadCsvAsync(key, progress, cancellationToken));
        }

        protected abstract UniTask<string> LoadCsvAsync(string key, IProgress<float> progress, CancellationToken cancellationToken);
    }
}