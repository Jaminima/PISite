using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    public static class Listener
    {
        #region Fields

        private static HttpListener _listener;

        #endregion Fields

        #region Methods

        private static async Task FinishReq(HttpListenerContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            middlewares.ForEach(x => x(context));
            var reqContext = new RequestContext(context);
            var route = new Route(reqContext);

            bool hitCache = false;

            context.Response.Headers.Add("Access-Control-Allow-Methods", "OPTIONS, GET, HEAD, DELETE, POST, PUT");
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            context.Response.Headers["Server"] = "PISite - ";

            if (route.method == "OPTIONS")
            {
                reqContext.SafeWriteString("");
            }
            else if (RequestRegistrar.Find(route, out var requestFunc))
            {
                if (context.Request.HttpMethod == "HEAD")
                {
                    reqContext.SafeWriteString("");
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
                        try
                        {
                            res = await requestFunc.callback(reqContext);
                            RequestCache.Store(requestFunc, res, route);
                        }
                        catch (Exception ex)
                        {
                            res = new ResponseState()
                            {
                                status = 500,
                                message = "Something has gone awfully wrong",
                                data = ex.ToString()
                            };
                        }
                    }

                    res.Send(reqContext);
                }
            }
            else
            {
                var expires = (DateTime.Now + new TimeSpan(1, 0, 0)).ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";

                context.Response.Headers.Add("Expires", expires);

                if (FileServer.FileExists(route, out var isGz))
                {
                    switch (context.Request.HttpMethod)
                    {
                        case "GET":
                            var findFile = await FileServer.Find(route, reqContext, isGz);
                            hitCache = findFile.hitCache;
                            break;

                        case "HEAD":
                            reqContext.SafeWriteString("");
                            break;

                        default:
                            context.Response.StatusCode = 405;

                            reqContext.SafeWriteObject(new { message = "405 - Illegal Method" });

                            break;
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                    var findFile = await FileServer.Find(new Route("GET", "", ""), reqContext, true);
                    hitCache = findFile.hitCache;
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

        #endregion Methods

        public static Action<HttpListenerContext, long, bool> log;
        public static List<Action<HttpListenerContext>> middlewares = new List<Action<HttpListenerContext>>();

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
    }
}