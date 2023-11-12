using ReusableTasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    internal class CachedItem
    {
        public ResponseState response;
        public RequestFunc requestFunc;
        public DateTime cachedAt;

        public CachedItem(ResponseState response, RequestFunc requestFunc)
        {
            this.response = response;
            this.requestFunc = requestFunc;
            this.cachedAt = DateTime.UtcNow;
        }
    }

    internal static class RequestCache
    {
        private static ConcurrentDictionary<string, CachedItem> cache = new ConcurrentDictionary<string, CachedItem>();

        public static bool Hit(RequestFunc requestFunc, out CachedItem cachedItem) { 
            var found = cache.TryGetValue(requestFunc.GetKey(), out cachedItem);

            if (!found)
                return false;

            if (cachedItem.cachedAt.Add(requestFunc.cacheFor) < DateTime.UtcNow)
            {
                cachedItem = null;
                cache.TryRemove(requestFunc.GetKey(), out _);
                return false;
            }

            return true;
        }

        public static void Store(RequestFunc requestFunc, ResponseState responseState)
        {
            if (cache.TryGetValue(requestFunc.GetKey(), out _))
            {
                return;
            }

            cache.TryAdd(requestFunc.GetKey(), new CachedItem(responseState, requestFunc));
        }
    }
}
