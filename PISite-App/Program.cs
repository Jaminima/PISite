using UnitsNet;
using Iot.Device.CpuTemperature;

namespace PISite_App
{
    internal class Program
    {
        static CpuTemperature temperature = new CpuTemperature();
        static void Main(string[] args)
        {
            while (true)
            {
                if (temperature.IsAvailable)
                {
                    Console.WriteLine($"The CPU temperature is {temperature.Temperature.DegreesCelsius}");
                }
                Thread.Sleep(2000); // sleep for 2000 milliseconds, 2 seconds
            }
        }
    }
}