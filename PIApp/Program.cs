using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace PIApp_Lib
{
    internal class Program
    {
        #region Methods

        private static async Task<ResponseState> A(RequestContext context)
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