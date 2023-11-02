using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    public static class RequestRegistrar
    {
        #region Fields

        private static List<RequestFunc> requestFuncs = new List<RequestFunc>();

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

        public Func<RequestContext, Task<ResponseState>> callback;
        public Route route;

        #endregion Fields

        #region Constructors

        public RequestFunc(Route route, Func<RequestContext, Task<ResponseState>> callback)
        {
            this.callback = callback;
            this.route = route;
        }

        public RequestFunc(string path, string method, Func<RequestContext, Task<ResponseState>> callback)
        {
            this.callback = callback;
            this.route = new Route() { method = method, path = path };
        }

        #endregion Constructors
    }

    public class ResponseState
    {
        #region Fields

        public object data;
        public string message;
        public int status = 200;

        #endregion Fields
    }

    public class Route
    {
        #region Fields

        public string method;
        public string path;

        #endregion Fields

        #region Methods

        public bool RouteMatch(Route enemy)
        {
            return path == enemy.path && method == enemy.method;
        }

        #endregion Methods
    }
}