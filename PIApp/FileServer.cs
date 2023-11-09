using System.IO;
using System.Linq;
using System.Net;

namespace PIApp_Lib
{
    public class FileServer
    {
        #region Fields

        public static string filePath = "./site";

        #endregion Fields

        #region Methods

        public static bool Find(Route route, HttpListenerContext context, StreamWriter writer)
        {
            if (route.method != "GET")
                return false;

            string trimmed_file = route.path.TrimStart('/');

            trimmed_file = trimmed_file.Length == 0 ? "index.html" : trimmed_file;

            string fileSrc = filePath + "/" + trimmed_file;

            if (File.Exists(fileSrc))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrc.Split('/').Last());

                using (var fs = new FileStream(fileSrc, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(writer.BaseStream);
                    fs.Flush();
                    fs.Close();
                }

                //Console.WriteLine($"Returned File {fileSrc}");
                return true;
            }
            return false;
        }

        #endregion Methods
    }
}