namespace MK.Data
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    internal sealed class CsvStorage : IStorage
    {
        IData IStorage.Load(Type type)
        {
            throw new NotImplementedException();
        }

        UniTask<IData> IStorage.LoadAsync(Type type, IProgress<float> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IStorage.Save(Type type)
        {
            throw new NotImplementedException();
        }
    }
}