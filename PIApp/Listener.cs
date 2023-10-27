using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIApp
{
    static internal class Listener
    {
        private static HttpListener _listener;

        private static void ReqBegin(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            _listener.BeginGetContext(ReqBegin, null);

            Console.WriteLine($"Request Received: {context.Request.HttpMethod} - {context.Request.RawUrl}");

            var writer = new StreamWriter(context.Response.OutputStream);

            var route = new Route() { path = context.Request.RawUrl, method = context.Request.HttpMethod };

            if (RequestRegistrar.Find(route, out var requestFunc))
            {

                var res = requestFunc.callback(context);

                context.Response.StatusCode = res.status;

                writer.Write(Jil.JSON.Serialize(res.data));

                //writer.Write(JObject.FromObject(res.data).ToString(Newtonsoft.Json.Formatting.None));
            }
            else if (FileServer.Find(route, context))
            {
                context.Response.StatusCode = 200;
            }
            else
            {
                context.Response.StatusCode = 404;

                writer.Write(Jil.JSON.Serialize(new { message = "Unable To Locate Path" }));
                //writer.Write(JObject.FromObject(new { message = "Unable To Locate Path" }).ToString(Newtonsoft.Json.Formatting.None));
            }
            writer.Flush();
            writer.Close();
        }

        public static void Init(int port = 8080)
        {
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
