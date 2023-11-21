namespace PIApp_Lib
{
    public class Route
    {
        #region Fields

        public string method;
        public string path;

        #endregion Fields

        #region Methods

        public bool RouteMatch(Route enemy)
        {
            return path == enemy.path && (method == enemy.method || enemy.method == "HEAD");
        }

        #endregion Methods
    }
}