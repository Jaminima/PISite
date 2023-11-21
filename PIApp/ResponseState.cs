using System.IO;
using System.Text;

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

        public void Send(RequestContext context)
        {
            var response = context.context.Response;

            response.StatusCode = status;
            response.ContentType = "application/json";

            var s = "";

            if (onlyData)
            {
                if (data.GetType() == typeof(string))
                {
                    s = data.ToString();
                    response.ContentType = "text/plain";
                }
                else
                    context.SafeWriteObject(data);
            }
            else
            {
                context.SafeWriteObject(new { data = data, message = message, status = status });
            }

            //response.ContentLength64 = s.Length;
        }
    }
}