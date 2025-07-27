#if MK_CLOUD_DATA
namespace MK.Data
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using MK.Log;

    internal sealed class JsonCloudStorage : BaseJsonStorage<ICloudStorage>
    {
        public JsonCloudStorage(ILoggerManager loggerManager) : base(loggerManager)
        {
        }

        protected override UniTask OnLoadAllAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override UniTask OnSaveAllAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override UniTask SaveAndSerializeAsync(string key, IData data, IProgress<float> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override UniTask<string> LoadJsonAsync(Type type, string key, IProgress<float> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
#endif