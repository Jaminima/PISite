﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    public class RequestContext
    {
        #region Fields

        private StreamWriter outputStream;
        public HttpListenerContext context;

        #endregion Fields

        #region Constructors

        public RequestContext(HttpListenerContext context)
        {
            this.context = context;
            this.outputStream = new StreamWriter(context.Response.OutputStream);
        }

        #endregion Constructors

        #region Methods

        public string GetBody()
        {
            if (context.Request.HttpMethod == "GET")
                throw new Exception("This Method Does Not Support A Body!");

            var s = new StreamReader(this.context.Request.InputStream);
            var str = s.ReadToEnd();
            s.Close();
            return str;
        }

        public T GetBody<T>()
        {
            var s = GetBody();

            try
            {
                return Jil.JSON.Deserialize<T>(s, Jil.Options.IncludeInherited);
            }
            catch
            {
                return default(T);
            }
        }

        public bool TryGetBody<T>(out T body)
        {
            var s = GetBody();

            try
            {
                body = Jil.JSON.Deserialize<T>(s, Jil.Options.IncludeInherited);
                return true;
            }
            catch (Exception e)
            {
                body = default(T);
                return false;
            }
        }

        public bool SafeFlushClose()
        {
            try
            {
                this.outputStream.Flush();
                this.outputStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool SafeWrite(Func<StreamWriter, Task> writeFunc)
        {
            try
            {
                var t = writeFunc(this.outputStream);
                t.Wait();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SafeWriteObject(object data)
        {
            var s = Jil.JSON.SerializeDynamic(data, Jil.Options.IncludeInherited);

            return SafeWriteString(s);
        }

        public bool SafeWriteString(string str)
        {
            return SafeWrite(x => outputStream.WriteAsync(str));
        }

        #endregion Methods
    }
}