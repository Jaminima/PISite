using System;
using System.Globalization;
using System.Net;
using ReusableTasks;

namespace PIApp_Lib
{
    internal class Program
    {
        #region Methods

        private static async ReusableTask<ResponseState> A(RequestContext context)
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