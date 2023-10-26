using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIApp
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World");
            Listener.Init();

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
