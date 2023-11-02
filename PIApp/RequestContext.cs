using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    public class RequestContext
    {
        public HttpListenerContext context;

        public RequestContext(HttpListenerContext context) 
        { 
            this.context = context;
        }

        public string GetBody()
        {
            var s = new StreamReader(this.context.Request.InputStream);
            var str = s.ReadToEnd();
            s.Close();
            return str;
        }
    }
}
