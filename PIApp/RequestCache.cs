using System;
using System.Collections.Concurrent;

namespace PIApp_Lib
{
    internal static class RequestCache
    {
        #region Fields

        private static ConcurrentDictionary<string, CachedItem> cache = new ConcurrentDictionary<string, CachedItem>();

        #endregion Fields

        #region Methods

        public static bool Hit(RequestFunc requestFunc, out CachedItem cachedItem)
        {
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

        public static void Store(RequestFunc requestFunc, ResponseState responseState, Route route = null)
        {
            if (route != null)
                requestFunc.route = route;

            if (cache.TryGetValue(requestFunc.GetKey(), out _))
            {
                return;
            }

            cache.TryAdd(requestFunc.GetKey(), new CachedItem(responseState, requestFunc));
        }

        #endregion Methods
    }

    internal class CachedItem
    {
        #region Fields

        public DateTime cachedAt;
        public RequestFunc requestFunc;
        public ResponseState response;

        #endregion Fields

        #region Constructors

        public CachedItem(ResponseState response, RequestFunc requestFunc)
        {
            this.response = response;
            this.requestFunc = requestFunc;
            this.cachedAt = DateTime.UtcNow;
        }

        #endregion Constructors
    }
}