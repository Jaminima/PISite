using System.IO;

namespace PIApp_Lib
{
    public class ResponseState
    {
        #region Fields

        public object data;
        public string message;
        public int status = 200;
        public bool onlyData = false;

        #endregion Fields

        public async void Send(RequestContext context)
        {
            var response = context.context.Response;

            response.StatusCode = status;
            response.ContentType = "application/json";
            response.SendChunked = false;

            var s = "";

            if (onlyData)
            {
                if (data.GetType() == typeof(string))
                {
                    s = data.ToString();
                    response.ContentType = "text/plain";
                }
                else
                    s = Jil.JSON.SerializeDynamic(data, Jil.Options.IncludeInherited);
            }
            else
            {
                s = Jil.JSON.SerializeDynamic(new {data = data, message = message, status = status}, Jil.Options.IncludeInherited);
            }

            response.ContentLength64 = s.Length;

            await context.SafeWrite(async x=>await x.WriteAsync(s));
        }
    }
}