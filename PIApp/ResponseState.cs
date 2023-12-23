namespace PIApp_Lib
{
    public class ResponseState
    {
        #region Fields

        public object data;
        public string message;
        public bool onlyData = false;
        public int status = 200;

        #endregion Fields

        #region Methods

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
                    response.ContentType = "text/plain";
                    context.SafeWriteString(data.ToString());
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

        #endregion Methods
    }
}