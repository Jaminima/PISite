using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    internal class Program
    {
        static ResponseState A(HttpListenerContext context)
        {
            Console.WriteLine("A");
            return new ResponseState()
            {
                message = "A",
                data = new { DogName = "Oscar" }
            };
        }


        static void Main(string[] args)
        {
            RequestRegistrar.Register(new RequestFunc("/api/test", "GET",A));

            Listener.Init();

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
