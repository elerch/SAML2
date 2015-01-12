using System;
using Microsoft.Owin.Hosting;

namespace SelfHostOwinSPExample
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "SelfHost Sample";

            const string url = "https://localhost:44333/core";
            using (WebApp.Start<Startup>(url)) {
                Console.WriteLine("\n\nServer listening at {0}. Press enter to stop", url);
                Console.ReadLine();
            }
        }
    }
}
