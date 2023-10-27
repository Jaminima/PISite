using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIApp
{
    public class ResponseState
    {
        public string message;
        public object data;
        public int status = 200;
    }

    public class Route
    {
        public string path;
        public string method;

        public bool RouteMatch(Route enemy)
        {
            return path == enemy.path && method == enemy.method;
        }
    }

    public class RequestFunc
    {
        public Func<HttpListenerContext, ResponseState> callback;
        public Route route;

        public RequestFunc(Route route, Func<HttpListenerContext, ResponseState> callback)
        {
            this.callback = callback;
            this.route = route;
        }

        public RequestFunc(string path, string method, Func<HttpListenerContext, ResponseState> callback)
        {
            this.callback = callback;
            this.route = new Route() { method = method, path = path };
        }
    }

    public static class RequestRegistrar
    {
        private static List<RequestFunc> requestFuncs = new List<RequestFunc>();

        public static void Register(RequestFunc requestFunc)
        {
            if (requestFuncs.Any(x=>x.route.RouteMatch(requestFunc.route)))
                throw new Exception("Path Already Registered");

            requestFuncs.Add(requestFunc);
        }

        public static bool Find(Route route, out RequestFunc requestFunc)
        {
            requestFunc = requestFuncs.Find(x => x.route.RouteMatch(route));

            return requestFunc != null;
        }
    }
}
