namespace MK.Data
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using MK.Log;
    using Newtonsoft.Json;

    internal abstract class BaseJsonStorage<TInterface> : BaseStorage<TInterface, IJsonData> where TInterface : IStorage
    {
        protected BaseJsonStorage(ILoggerManager loggerManager) : base(loggerManager)
        {
        }

        protected override async UniTask<IData> DeserializeAsync(string key, Type type, IProgress<float> progress, CancellationToken cancellationToken)
        {
            return (IData)JsonConvert.DeserializeObject(await this.LoadJsonAsync(key, type, progress, cancellationToken), type);
        }

        protected abstract UniTask<string> LoadJsonAsync(string key, Type type, IProgress<float> progress, CancellationToken cancellationToken);
    }
}