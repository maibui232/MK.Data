namespace MK.Data
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using MK.AssetsManager;
    using MK.Log;
    using UnityEngine;

    internal sealed class CsvLocalStorage : BaseCsvStorage<ILocalStorage>
    {
        private readonly IAssetsManager assetsManager;

        public CsvLocalStorage(ILoggerManager loggerManager, CsvStreamer csvStreamer, IAssetsManager assetsManager) : base(loggerManager, csvStreamer)
        {
            this.assetsManager = assetsManager;
        }

        protected override UniTask OnLoadAllAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask OnSaveAllAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override async UniTask<string> LoadCsvAsync(string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return (await this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)).text;
        }

        protected override UniTask SaveAndSerializeAsync(string key, IData data, IProgress<float> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("not implemented save method");
        }
    }
}