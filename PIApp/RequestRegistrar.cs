﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ReusableTasks;

namespace PIApp_Lib
{
    public static class RequestRegistrar
    {
        #region Fields

        public static List<RequestFunc> requestFuncs = new List<RequestFunc>();

        #endregion Fields

        #region Methods

        public static bool Find(Route route, out RequestFunc requestFunc)
        {
            requestFunc = requestFuncs.Find(x => x.route.RouteMatch(route));

            return requestFunc != null;
        }

        public static void Register(RequestFunc requestFunc)
        {
            if (requestFuncs.Any(x => x.route.RouteMatch(requestFunc.route)))
                throw new Exception("Path Already Registered");

            requestFuncs.Add(requestFunc);
        }

        #endregion Methods
    }

    public class RequestFunc
    {
        #region Fields

        public Func<RequestContext, ReusableTask<ResponseState>> callback;
        public Route route;
        public TimeSpan cacheFor = TimeSpan.Zero;

        #endregion Fields

        #region Constructors

        public RequestFunc(Route route, Func<RequestContext, ReusableTask<ResponseState>> callback)
        {
            this.callback = callback;
            this.route = route;
        }

        public RequestFunc(Route route, Func<RequestContext, ReusableTask<ResponseState>> callback, TimeSpan cacheFor)
        {
            this.callback = callback;
            this.route = route;
            this.cacheFor = cacheFor;
        }

        public RequestFunc(string path, string method, Func<RequestContext, ReusableTask<ResponseState>> callback)
        {
            this.callback = callback;
            this.route = new Route() { method = method, path = path };
        }

        public RequestFunc(string path, string method, Func<RequestContext, ReusableTask<ResponseState>> callback, TimeSpan cacheFor)
        {
            this.callback = callback;
            this.route = new Route() { method = method, path = path };
            this.cacheFor = cacheFor;
        }

        #endregion Constructors

        public string GetKey()
        {
            return $"{this.route.path}-{this.route.method}";
        }
    }
}