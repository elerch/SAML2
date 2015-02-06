using System;
using Microsoft.Owin.Hosting;

namespace SelfHostOwinSPExample
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "SelfHost Sample";
            var url = "https://localhost:44333/core";
#if TEST
            url = "http://localhost:7777/identity";
#endif
            using (WebApp.Start<Startup>(url)) {
                Console.WriteLine("\n\nServer listening at {0}. Press enter to stop", url);
                Console.ReadLine();
            }
        }
    }
}
