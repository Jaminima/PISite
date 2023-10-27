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
        public static bool Alive = false;
        private static string[] Files = new string[0];
        private static DateTime lastFilesLoad = DateTime.MinValue;

        public static void UpdateFiles()
        {
            var now = DateTime.UtcNow;
            if (lastFilesLoad.AddSeconds(10) > now)
            {
                return;
            }
            lastFilesLoad = now;

            Files = Directory.GetFiles(filePath).Select(x => x.Substring(filePath.Length + 1)).ToArray();

            Console.WriteLine($"Tracking {Files.Length} Files");
        }

        public static void Init()
        {
            if (!Directory.Exists(filePath))
                return;

            Alive = true;
            UpdateFiles();
        }

        public static bool Find(Route route, HttpListenerContext context)
        {
            if (route.method != "GET")
                return false;

            UpdateFiles();

            if (Files.Contains(route.path.TrimStart('/')))
            {
                string fileSrc = filePath + route.path;
                using (var fs = new FileStream(fileSrc, FileMode.Open))
                {
                    fs.CopyTo(context.Response.OutputStream);
                }

                context.Response.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileSrc);
                Console.WriteLine($"Returned File {route.path}");
                return true;
            }
            return false;
        }
    }
}
