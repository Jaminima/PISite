using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;

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

        private static async Task FinishReq(HttpListenerContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            middlewares.ForEach(x => x(context));
            var route = new Route() { path = context.Request.RawUrl, method = context.Request.HttpMethod };
            var reqContext = new RequestContext(context);

            bool hitCache = false;

            if (RequestRegistrar.Find(route, out var requestFunc))
            {
                if (context.Request.HttpMethod == "HEAD")
                {
                    await reqContext.SafeWrite(async x => await x.WriteAsync(""));
                }
                else
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
            }
            else
            {
                var expires = (DateTime.UtcNow + new TimeSpan(1, 0, 0)).ToString("dddd, dd MMM yyyy HH:mm:ss") + " GMT";

                context.Response.Headers.Add("Expires", expires);

                if (FileServer.FileExists(route))
                {
                    switch (context.Request.HttpMethod)
                    {
                        case "GET":
                            var findFile = await FileServer.Find(route, reqContext);
                            hitCache = findFile.hitCache;
                            break;

                        case "HEAD":
                            await reqContext.SafeWrite(async x => await x.WriteAsync(""));
                            break;

                        default:
                            context.Response.StatusCode = 405;

                            var s = Jil.JSON.Serialize(new { message = "405 - Illegal Method" });

                            await reqContext.SafeWrite(async x => await x.WriteAsync(s));

                            break;
                    }
                }
                else
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

            if (Debugger.IsAttached)
                _listener.Prefixes.Add($"http://localhost:{port}/");
            else
                _listener.Prefixes.Add($"http://+:{port}/");

            _listener.Start();
            _listener.BeginGetContext(ReqBegin, null);
        }

        #endregion Methods
    }
}