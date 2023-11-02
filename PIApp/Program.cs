using System;
using System.Net;

namespace PIApp_Lib
{
    internal class Program
    {
        #region Methods

        private static ResponseState A(HttpListenerContext context)
        {
            Console.WriteLine("A");
            return new ResponseState()
            {
                message = "A",
                data = new { DogName = "Oscar" }
            };
        }

        private static void Main(string[] args)
        {
            RequestRegistrar.Register(new RequestFunc("/api/test", "GET", A));

            Listener.Init();

            while (true)
            {
                Console.ReadLine();
            }
        }

        #endregion Methods
    }
}