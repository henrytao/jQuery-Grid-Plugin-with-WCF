using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;

namespace EZTier.LogServiceHost
{
    class Start
    {
        static void Main(string[] args)
        {
         
            /*
            WebScriptServiceHostFactory factory = new WebScriptServiceHostFactory();
            Uri[] uris = new Uri[]{ new Uri("")};
            var host = factory.CreateServiceHost(typeof(LogServiceHost.LoggerService), uris);
            */

            using (ServiceHost host = new ServiceHost(typeof(LogServiceHost.LoggerService)))
            {
                host.Open();

                Console.WriteLine();
                Console.WriteLine("Log Service listening on port 8168");
                Console.WriteLine("Press <ENTER> to terminate Host");
                Console.ReadLine();
            }
            

            /*
            using (WebServiceHost host = new WebServiceHost(typeof(LogServiceHost.LoggerService), new Uri("http://localhost:8168/LoggerService")))
            {
                host.Open();

                Console.WriteLine();
                Console.WriteLine("Log Service listening on port 8168");
                Console.WriteLine("Press <ENTER> to terminate Host");
                Console.ReadLine();

            };
           */
        }
    }
}
