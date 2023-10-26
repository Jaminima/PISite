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

    public class RequestFunc
    {
        public Func<HttpListenerContext, ResponseState> callback;

        public string path;

        public RequestFunc(string path, Func<HttpListenerContext, ResponseState> callback)
        {
            this.callback = callback;
            this.path = path;
        }
    }

    public static class RequestRegistrar
    {
        private static Dictionary<string, RequestFunc> requestFuncs = new Dictionary<string, RequestFunc>();

        public static void Register(RequestFunc requestFunc)
        {
            if (requestFuncs.ContainsKey(requestFunc.path))
                throw new Exception("Path Already Registered");

            requestFuncs.Add(requestFunc.path, requestFunc);
        }

        public static bool Find(string path, out RequestFunc requestFunc)
        {
            return requestFuncs.TryGetValue(path, out requestFunc);
        }
    }
}
