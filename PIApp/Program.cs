using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PIApp
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
            Console.WriteLine("Hello World");

            RequestRegistrar.Register(new RequestFunc("/", "GET",A));

            Listener.Init();

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
