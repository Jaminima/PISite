using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;

namespace PIApp_Lib
{
    public class FileServer
    {
        #region Fields

        public static string filePath = "./site";

        private static ConcurrentDictionary<string, string> cachedFiles = new ConcurrentDictionary<string, string>();

        #endregion Fields

        #region Methods

        public static bool Find(Route route, HttpListenerContext context, StreamWriter writer)
        {
            if (route.method != "GET")
                return false;

            string trimmed_file = route.path.TrimStart('/');

            trimmed_file = trimmed_file.Length == 0 ? "index.html" : trimmed_file;

            string fileSrc = filePath + "/" + trimmed_file;

            if (cachedFiles.TryGetValue(trimmed_file, out var content))
            {
                writer.Write(content);
            }
            else if (File.Exists(fileSrc))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrc.Split('/').Last());

                using (var fs = new FileStream(fileSrc, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var s = new StreamReader(fs).ReadToEnd();

                    cachedFiles.TryAdd(trimmed_file, s);

                    fs.Position = 0;
                    fs.CopyTo(writer.BaseStream);
                    fs.Flush();
                    fs.Close();
                }
                return true;
            }
            return false;
        }

        #endregion Methods
    }
}