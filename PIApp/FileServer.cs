using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIApp
{
    internal class FileServer
    {
        private static string filePath = "./Site";

        public static bool Find(Route route, HttpListenerContext context)
        {
            if (route.method != "GET")
                return false;

            string trimmed_file = route.path.TrimStart('/');

            trimmed_file = trimmed_file.Length == 0 ? "index.html" : trimmed_file;

            string fileSrc = filePath + "/" + trimmed_file;

            if (File.Exists(fileSrc))
            {
                using (var fs = new FileStream(fileSrc, FileMode.Open))
                {
                    fs.CopyTo(context.Response.OutputStream);
                }

                context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrc.Split('/').Last());
                Console.WriteLine($"Returned File {route.path}");
                return true;
            }
            return false;
        }
    }
}
