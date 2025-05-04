namespace MK.Data
{
    using System;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    internal interface IStorage
    {
        IData Load(Type type);

        IData[] Load(params Type[] types)
        {
            return types.Select(this.Load).ToArray();
        }

        T Load<T>() where T : IData, IReadable => (T)this.Load(typeof(T));

        UniTask<IData> LoadAsync(Type type, IProgress<float> progress = null, CancellationToken cancellationToken = default);

        async UniTask<IData[]> LoadAsync(Type[] types, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            var tasks = Enumerable.Select(types, type => this.LoadAsync(type, cancellationToken: cancellationToken)).ToArray();
            await UniTask.WhenAll(tasks);

            return tasks.Select(x => x.GetAwaiter().GetResult()).ToArray();
        }

        async UniTask<T> LoadAsync<T>(IProgress<float> progress = null, CancellationToken cancellationToken = default) where T : IData
        {
            var data = await this.LoadAsync(typeof(T), progress, cancellationToken);

            return (T)data;
        }

        void Save(Type type);

        void Save(params Type[] types)
        {
            foreach (var type in types)
            {
                this.Save(type);
            }
        }

        void Save<T>() where T : IData, IWriteable => this.Save(typeof(T));
    }
}