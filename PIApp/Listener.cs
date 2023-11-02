using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime;

namespace PIApp_Lib
{
    public static class Listener
    {
        #region Fields

        private static HttpListener _listener;

        #endregion Fields

        #region Methods

        private static async void ReqBegin(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            _listener.BeginGetContext(ReqBegin, null);

            Console.WriteLine($"Request Received: {context.Request.HttpMethod} - {context.Request.RawUrl}");

            var route = new Route() { path = context.Request.RawUrl, method = context.Request.HttpMethod };

            if (RequestRegistrar.Find(route, out var requestFunc))
            {
                var writer = new StreamWriter(context.Response.OutputStream);

                var res = await requestFunc.callback(new RequestContext(context));

                context.Response.StatusCode = res.status;

                writer.Write(Jil.JSON.SerializeDynamic(new { data = res.data, message = res.message}));
                writer.Flush();
                writer.Close();
            }
            else if (FileServer.Find(route, context))
            {
                context.Response.StatusCode = 200;
            }
            else
            {
                var writer = new StreamWriter(context.Response.OutputStream);

                context.Response.StatusCode = 404;

                writer.Write(Jil.JSON.Serialize(new { message = "Unable To Locate Path" }));
                writer.Flush();
                writer.Close();
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