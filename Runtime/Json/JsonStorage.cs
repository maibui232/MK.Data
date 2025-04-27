namespace MK.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using MK.AssetsManager;
    using MK.Extensions;
    using MK.Log;
    using Newtonsoft.Json;
    using UnityEngine;
    using ILogger = MK.Log.ILogger;

    public class JsonStorage : IStorage
    {
        private readonly IAssetsManager            assetsManager;
        private readonly ILogger                   logger;
        private readonly Dictionary<string, IData> keyToData = new();

        public JsonStorage(IAssetsManager assetsManager, ILoggerManager loggerManager)
        {
            this.assetsManager = assetsManager;
            this.logger        = loggerManager.GetLogger(this);
        }

        private IData AddNew(string newKey, IData newData)
        {
            this.keyToData.Add(newKey, newData);

            return newData;
        }

        IData IStorage.Load(Type type)
        {
            this.logger.ThrowIfNotAssignable(type, typeof(IReadable));
            var key = type.GetKey();

            if (this.keyToData.TryGetValue(key, out var data)) return data;
            string json;
            if (type.IsSubclassOf(typeof(IWriteable)))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    json = PlayerPrefs.GetString(key);
                }
                else
                {
                    return this.AddNew(key, (IData)Activator.CreateInstance(type));
                }
            }
            else
            {
                json = this.assetsManager.Load<TextAsset>(key).text;
            }

            return this.AddNew(key, (IData)JsonConvert.DeserializeObject(json, type));
        }

        async UniTask<IData> IStorage.LoadAsync(Type type, IProgress<float> progress, CancellationToken cancellationToken)
        {
            this.logger.ThrowIfNotAssignable(type, typeof(IReadable));
            var key = type.GetKey();

            if (this.keyToData.TryGetValue(key, out var data)) return data;
            string json;
            if (type.IsSubclassOf(typeof(IWriteable)))
            {
                if (PlayerPrefs.HasKey(key))
                {
                    json = PlayerPrefs.GetString(key);
                }
                else
                {
                    return this.AddNew(key, (IData)Activator.CreateInstance(type));
                }
            }
            else
            {
                json = (await this.assetsManager.LoadAsync<TextAsset>(key, cancellationToken: cancellationToken)).text;
            }

            return this.AddNew(key, (IData)JsonConvert.DeserializeObject(json, type));
        }

        void IStorage.Save(Type type)
        {
            this.logger.ThrowIfNotAssignable(type, typeof(IWriteable));
            var key = type.GetKey();
            if (!this.keyToData.TryGetValue(key, out var data))
            {
                data = ((IStorage)this).Load(type);
            }

            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(data));
        }
    }
}