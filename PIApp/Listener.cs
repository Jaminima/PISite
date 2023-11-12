using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using ReusableTasks;

namespace PIApp_Lib
{


    public static class Listener
    {
        #region Fields

        private static HttpListener _listener;

        public static List<Action<HttpListenerContext>> middlewares = new List<Action<HttpListenerContext>>();

        public static Action<HttpListenerContext,long, bool> log;

        #endregion Fields

        #region Methods

        private static async ReusableTask FinishReq(HttpListenerContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            middlewares.ForEach(x => x(context));
            var route = new Route() { path = context.Request.RawUrl, method = context.Request.HttpMethod };
            var reqContext = new RequestContext(context);

            bool hitCache = false;

            if (RequestRegistrar.Find(route, out var requestFunc))
            {
                ResponseState res;

                if (RequestCache.Hit(requestFunc, out var cachedItem))
                {
                    res = cachedItem.response;
                    hitCache = true;
                }
                else
                {
                    res = await requestFunc.callback(reqContext);
                    RequestCache.Store(requestFunc, res);
                }

                res.Send(reqContext);
            }
            else
            {
                var findFile = await FileServer.Find(route, reqContext);

                hitCache = findFile.hitCache;

                if (!findFile.found)
                {
                    context.Response.StatusCode = 404;

                    var s = Jil.JSON.Serialize(new { message = "404 - Unable To Locate Path" });

                    await reqContext.SafeWrite(async x => await x.WriteAsync(s));
                }
            }

            reqContext.SafeFlushClose();

            stopwatch.Stop();
            var ms = stopwatch.ElapsedMilliseconds;

            if (log != null)
                log(context, ms, hitCache);
        }

        private static async void ReqBegin(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            _listener.BeginGetContext(ReqBegin, null);

            await FinishReq(context);
        }

        public static void Init(int port = 8080)
        {
            Console.WriteLine($"Starting Up HTTP Server On {port}");

            _listener = new HttpListener();

            if (Debugger.IsAttached || true)
                _listener.Prefixes.Add($"http://localhost:{port}/");
            else
                _listener.Prefixes.Add($"http://+:{port}/");

            _listener.Start();
            _listener.BeginGetContext(ReqBegin, null);
        }

        #endregion Methods
    }
}