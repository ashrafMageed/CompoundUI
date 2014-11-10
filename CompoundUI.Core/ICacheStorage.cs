using System;

namespace CompoundUI.Core
{
    public interface ICacheStorage
    {
        T Get<T>(string key, Func<T> getWhenCacheMiss);
    }
}