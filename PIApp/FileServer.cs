using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace PIApp_Lib
{
    public class FileServer
    {
        #region Fields

        public static string filePath = "./site";

        private static ConcurrentDictionary<string, byte[]> cachedFiles = new ConcurrentDictionary<string, byte[]>();

        #endregion Fields

        #region Methods

        public static bool Find(Route route, HttpListenerContext context, StreamWriter writer, out bool hitCache)
        {
            hitCache = false;

            if (route.method != "GET")
                return false;

            string trimmed_file = route.path.TrimStart('/');

            trimmed_file = trimmed_file.Length == 0 ? "index.html" : trimmed_file;

            string fileSrc = filePath + "/" + trimmed_file;

            if (cachedFiles.TryGetValue(trimmed_file, out var content))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrc.Split('/').Last());

                writer.BaseStream.Write(content, 0, content.Length);
                hitCache = true;
                return true;
            }
            else if (File.Exists(fileSrc))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrc.Split('/').Last());

                using (var fs = new FileStream(fileSrc, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bytes = new byte[fs.Length];

                    fs.Read(bytes, 0, (int)fs.Length);

                    fs.Close();

                    writer.BaseStream.Write(bytes, 0, bytes.Length);

                    cachedFiles.TryAdd(trimmed_file, bytes);
                }
                return true;
            }
            return false;
        }

        #endregion Methods
    }
}