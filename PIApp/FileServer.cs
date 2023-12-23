using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    public class FileServer
    {
        #region Fields

        private static ConcurrentDictionary<string, byte[]> cachedFiles = new ConcurrentDictionary<string, byte[]>();
        public static string filePath = "./site";

        #endregion Fields

        #region Methods

        public static bool FileExists(Route route, out bool isGz)
        {
            string trimmed_file = route.path.TrimStart('/');

            trimmed_file = trimmed_file.Length == 0 ? "index.html" : trimmed_file;

            string fileSrc = filePath + "/" + trimmed_file;

            isGz = cachedFiles.ContainsKey(fileSrc + ".gz") || File.Exists(fileSrc + ".gz");

            return cachedFiles.ContainsKey(fileSrc) || File.Exists(fileSrc);
        }

        public static async Task<FileFindResponse> Find(Route route, RequestContext context, bool isGz)
        {
            if (route.method != "GET")
                return new FileFindResponse() { found = false, hitCache = false };

            string trimmed_file = route.path.TrimStart('/');

            trimmed_file = trimmed_file.Length == 0 ? "index.html" : trimmed_file;

            string fileSrc = filePath + "/" + trimmed_file;
            string fileSrcNoGz = fileSrc;

            if (isGz)
                fileSrc += ".gz";

            if (cachedFiles.TryGetValue(trimmed_file, out var content))
            {
                context.context.Response.StatusCode = 200;
                context.context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrcNoGz.Split('/').Last());
                if (isGz)
                    context.context.Response.AddHeader("Content-Encoding", "gzip");

                context.SafeWrite(x => x.BaseStream.WriteAsync(content, 0, content.Length));

                return new FileFindResponse() { found = true, hitCache = true };
            }
            else if (File.Exists(fileSrc))
            {
                context.context.Response.StatusCode = 200;
                context.context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrcNoGz.Split('/').Last());
                if (isGz)
                    context.context.Response.AddHeader("Content-Encoding", "gzip");

                using (var fs = new FileStream(fileSrc, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bytes = new byte[fs.Length];

                    await fs.ReadAsync(bytes, 0, (int)fs.Length);

                    fs.Close();

                    context.SafeWrite(x => x.BaseStream.WriteAsync(bytes, 0, bytes.Length));

                    cachedFiles.TryAdd(trimmed_file, bytes);
                }
                return new FileFindResponse() { found = true, hitCache = false };
            }
            return new FileFindResponse() { found = false, hitCache = false };
        }

        #endregion Methods

        #region Classes

        public class FileFindResponse
        {
            #region Fields

            public bool found;
            public bool hitCache;

            #endregion Fields
        }

        #endregion Classes
    }
}