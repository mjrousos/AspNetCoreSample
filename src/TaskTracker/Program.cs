using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace TaskList
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()

                // UseUrls can specify which URLs Kestrel listens to.
                // Notice that we can specify multiple URLs.
                //
                // http://+ will listen on all URLs, so it will enable the app to
                // listen for names other than localhost/127.0.0.1 (which will be 
                // useful once the app is deployed)
                // https://msdn.microsoft.com/en-us/library/aa364698(v=vs.85).aspx
                //
                // Note that + is preferred to 0.0.0.0 because * will listen on IP4 /and/ IP6 address instead of on
                // just IP4.
                //
                // This call MUST come before UseIISIntegration (if IIS integration is used)
                // 
                // URLs can also be picked up from a config file, if needed 
                // http://stackoverflow.com/questions/34212765/
                .UseUrls("http://+:5000", "http://+:5001")
                
                .UseContentRoot(Directory.GetCurrentDirectory())

                // Note that UseIISIntegration MUST come after UseUrls so that the URLs IIS integration sets aren't overridden
                .UseIISIntegration()

                .UseStartup<Startup>()  // Specify the startup class which will configure the application
                .Build();

            host.Run();
        }
    }
}
