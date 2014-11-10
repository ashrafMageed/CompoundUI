using System;
using System.Collections.Generic;

namespace CompoundUI.Core
{
    public sealed class InMemoryCacheStorage : ICacheStorage
    {
        private static readonly Lazy<InMemoryCacheStorage>  _lazy = new Lazy<InMemoryCacheStorage>(() => new InMemoryCacheStorage());

        private static readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

        public static InMemoryCacheStorage Instance { get { return _lazy.Value; }}

        private InMemoryCacheStorage() {}

        public T Get<T>(string key, Func<T> getWhenCacheMiss)
        {
            if (_storage.ContainsKey(key))
                return (T)_storage[key];

            var item = getWhenCacheMiss();
            _storage.Add(key, item);
            return item;
        }
    }
}