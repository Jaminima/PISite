using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    public class FileServer
    {
        #region Fields

        public static string filePath = "./site";

        private static ConcurrentDictionary<string, byte[]> cachedFiles = new ConcurrentDictionary<string, byte[]>();

        #endregion Fields

        public class FileFindResponse
        {
            public bool found;
            public bool hitCache;
        }

        #region Methods

        public static bool FileExists(Route route)
        {
            string trimmed_file = route.path.TrimStart('/');

            trimmed_file = trimmed_file.Length == 0 ? "index.html" : trimmed_file;

            string fileSrc = filePath + "/" + trimmed_file;

            return cachedFiles.ContainsKey(fileSrc) || File.Exists(fileSrc);
        }

        public static async Task<FileFindResponse> Find(Route route, RequestContext context)
        {
            if (route.method != "GET")
                return new FileFindResponse() { found = false, hitCache = false };

            string trimmed_file = route.path.TrimStart('/');

            trimmed_file = trimmed_file.Length == 0 ? "index.html" : trimmed_file;

            string fileSrc = filePath + "/" + trimmed_file;

            if (cachedFiles.TryGetValue(trimmed_file, out var content))
            {
                context.context.Response.StatusCode = 200;
                context.context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrc.Split('/').Last());

                await context.SafeWrite(async x=>await x.BaseStream.WriteAsync(content, 0, content.Length));

                return new FileFindResponse() { found = true, hitCache = true };
            }
            else if (File.Exists(fileSrc))
            {
                context.context.Response.StatusCode = 200;
                context.context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrc.Split('/').Last());

                using (var fs = new FileStream(fileSrc, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bytes = new byte[fs.Length];

                    await fs.ReadAsync(bytes, 0, (int)fs.Length);

                    fs.Close();

                    await context.SafeWrite(async x => await x.BaseStream.WriteAsync(bytes, 0, bytes.Length));

                    cachedFiles.TryAdd(trimmed_file, bytes);
                }
                return new FileFindResponse() { found = true, hitCache = false };
            }
            return new FileFindResponse() { found = false, hitCache = false };
        }

        #endregion Methods
    }
}