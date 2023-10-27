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

        public static void Init()
        {
            if (!Directory.Exists(filePath))
                return;

            Files = Directory.GetFiles(filePath);
            Alive = true;
        }

        public static bool Find(Route route, HttpListenerContext context)
        {
            return false;
        }
    }
}
