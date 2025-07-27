namespace MK.Data
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using MK.AssetsManager;
    using MK.Log;
    using Newtonsoft.Json;
    using UnityEngine;

    internal sealed class JsonLocalStorage : BaseJsonStorage<ILocalStorage>
    {
        private readonly IAssetsManager assetsManager;

        public JsonLocalStorage(IAssetsManager assetsManager, ILoggerManager loggerManager) : base(loggerManager)
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

        protected override async UniTask<string> LoadJsonAsync(string key, Type type, IProgress<float> progress, CancellationToken cancellationToken)
        {
            string json;
            if (type.IsSubclassOf(typeof(IWriteable)))
            {
                json = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : JsonConvert.SerializeObject(Activator.CreateInstance(type));
            }
            else
            {
                json = (await this.assetsManager.LoadAsync<TextAsset>(key, progress, cancellationToken)).text;
            }

            return json;
        }

        protected override UniTask SaveAndSerializeAsync(string key, IData data, IProgress<float> progress, CancellationToken cancellationToken)
        {
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(data));

            return UniTask.CompletedTask;
        }
    }
}