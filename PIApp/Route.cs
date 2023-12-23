namespace PIApp_Lib
{
    public class Route
    {
        #region Fields

        public string method;
        public string path;
        public string prams;

        #endregion Fields

        #region Constructors

        public Route(RequestContext context)
        {
            this.path = context.context.Request.Url.AbsolutePath;
            this.method = context.context.Request.HttpMethod;
            this.prams = context.context.Request.Url.Query;
        }

        public Route(string method, string path, string prams)
        {
            this.method = method;
            this.path = path;
            this.prams = prams;
        }

        #endregion Constructors

        #region Methods

        public bool RouteMatch(Route enemy)
        {
            return path == enemy.path && (method == enemy.method || enemy.method == "HEAD")/* && prams == enemy.prams*/;
        }

        #endregion Methods
    }
}