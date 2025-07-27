namespace MK.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using MK.Extensions;
    using MK.Log;
    using Unity.Plastic.Newtonsoft.Json;

    internal interface IStorage
    {
        UniTask LoadAllAsync(IProgress<float> progress, CancellationToken cancellationToken);

        UniTask<IData> LoadAsync(Type type, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        async UniTask<IData[]> LoadAsync(Type[] types, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            var tasks = Enumerable.Select(types, type => this.LoadAsync(type, progress, cancellationToken)).ToArray();
            await UniTask.WhenAll(tasks);

            return tasks.Select(x => x.GetAwaiter().GetResult()).ToArray();
        }

        async UniTask<T> LoadAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IData
        {
            return (T)await this.LoadAsync(typeof(T), progress, cancellationToken);
        }

        UniTask SaveAllAsync(IProgress<float> progress, CancellationToken cancellationToken = default);

        UniTask SaveAsync(Type type, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        UniTask SaveAsync(Type[] types, IProgress<float> progress = null, CancellationToken cancellationToken = default) => UniTask.WhenAll(types.Select(type => this.SaveAsync(type, progress, cancellationToken)));

        UniTask SaveAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IData, IWriteable => this.SaveAsync(typeof(T), progress, cancellationToken);
    }

    internal interface ILocalStorage : IStorage
    {
    }

    internal interface ICloudStorage : IStorage
    {
    }

    internal abstract class BaseStorage<TInterface, TData> : IStorage where TInterface : IStorage
    {
        private readonly ILogger                   logger;
        private readonly Dictionary<string, IData> keyToData = new();

        protected BaseStorage(ILoggerManager loggerManager)
        {
            this.logger = loggerManager.GetLogger(this);
        }

        private IData AddNew(string newKey, IData newData)
        {
            this.keyToData.Add(newKey, newData);

            return newData;
        }

        async UniTask IStorage.LoadAllAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(this.keyToData.Values.Select(data => ((IStorage)this).LoadAsync(data.GetType(), progress, cancellationToken)));
            this.OnLoadAllAsync(progress, cancellationToken);
        }

        async UniTask<IData> IStorage.LoadAsync(Type type, IProgress<float> progress, CancellationToken cancellationToken)
        {
            this.logger.ThrowIfNotAssignable(type, typeof(IReadable));
            var key = type.GetKey();

            return this.keyToData.TryGetValue(key, out var data)
                       ? data
                       : this.AddNew(key, await this.DeserializeAsync(key, type, progress, cancellationToken));
        }

        async UniTask IStorage.SaveAllAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(this.keyToData.Values.Select(data => ((IStorage)this).SaveAsync(data.GetType(), progress, cancellationToken)));
            await this.OnSaveAllAsync(progress, cancellationToken);
        }

        async UniTask IStorage.SaveAsync(Type type, IProgress<float> progress, CancellationToken cancellationToken)
        {
            this.logger.ThrowIfNotAssignable(type, typeof(IWriteable));
            var key = type.GetKey();
            if (!this.keyToData.TryGetValue(key, out var data))
            {
                data = await ((IStorage)this).LoadAsync(type, progress, cancellationToken);
            }

            await this.SaveAndSerializeAsync(key, data, progress, cancellationToken);
        }

        protected abstract UniTask OnLoadAllAsync(IProgress<float> progress, CancellationToken cancellationToken);

        protected abstract UniTask OnSaveAllAsync(IProgress<float> progress, CancellationToken cancellationToken);

        protected abstract UniTask<IData> DeserializeAsync(string key, Type type, IProgress<float> progress, CancellationToken cancellationToken);

        protected abstract UniTask SaveAndSerializeAsync(string key, IData data, IProgress<float> progress, CancellationToken cancellationToken);
    }
}