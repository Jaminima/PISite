using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIApp
{
    static internal class Listener
    {
        static HttpListener _listener;

        private static void ReqBegin(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);
            _listener.BeginGetContext(ReqBegin, null);

            Console.WriteLine($"Request Received: {context.Request.Url.ToString()}");
        }

        public static void Init(int port = 8080)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{port}/");
            _listener.Start();
            _listener.BeginGetContext(ReqBegin, null);
        }
    }
}
