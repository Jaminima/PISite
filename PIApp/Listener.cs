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

            var writer = new StreamWriter(context.Response.OutputStream);

            bool hitCache = false;

            if (RequestRegistrar.Find(route, out var requestFunc))
            {
                var res = await requestFunc.callback(new RequestContext(context));

                res.Send(context.Response, writer);
            }
            else
            {
                var findFile = await FileServer.Find(route, context, writer);

                hitCache = findFile.hitCache;

                if (!findFile.found)
                {
                    context.Response.StatusCode = 404;

                    await writer.WriteAsync(Jil.JSON.Serialize(new { message = "404 - Unable To Locate Path" }));
                }
            }

            try
            {
                await writer.FlushAsync();
                writer.Close();
            }
            catch
            {
                Console.WriteLine($"Req To {route.path} Failed");
            }

            stopwatch.Stop();
            var ms = stopwatch.ElapsedMilliseconds;

            if (log != null)
                log(context, ms, hitCache);
        }

        private static async void ReqBegin(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            _listener.BeginGetContext(ReqBegin, null);

            try
            {
                await FinishReq(context);
            }
            catch (Exception ex)
            {

            }
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