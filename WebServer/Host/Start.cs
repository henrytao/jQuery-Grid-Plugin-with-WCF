using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Dispatcher;
using System.Diagnostics;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Net;

using SHDocVw;

namespace EZTier.MVNet.Monitor
{
    class Start
    {
        static string logger_uri = "http://localhost:8080/index.html";

        static void Main(string[] args)
        {
            Process[] processes = Process.GetProcessesByName("EZTier.LogServiceHost");
            
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://+:8080/");
            listener.Start();

            Console.WriteLine(
              "Listening for requests on http://localhost:8080/");

            InvokeInternetExplorer();
            
            while (true)
            {
                HttpListenerContext ctx = listener.GetContext();

                string page = "";
                if (!ctx.Request.Url.LocalPath.ToLower().StartsWith("/js")
                    & !ctx.Request.Url.LocalPath.ToLower().StartsWith("/themes")
                    & !ctx.Request.Url.LocalPath.ToLower().StartsWith("/lib")
                    )
                {

                    page = ctx.Request.Url.LocalPath.Replace("/", "");
                }
                else
                {
                    page = Directory.GetCurrentDirectory() + ctx.Request.Url.LocalPath.Replace("/",@"\");
                }
                
                string query = ctx.Request.Url.Query.Replace("?", "");
                Console.WriteLine("Received request for {0}?{1}", page, query);

                HttpListenerResponse response = null;

                try
                {
                    // Create the response.
                    response = ctx.Response;

                    byte[] buffer = File.ReadAllBytes(page);

                    response.ContentLength64 = buffer.Length;

                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
                ctx.Response.Close();               
            }
        }

        static void InvokeInternetExplorer()
        {
            foreach (InternetExplorer ie in new ShellWindowsClass())
            {
                try
                {
                    if (ie.LocationURL.StartsWith("http://"))
                    {
                        Object o = null;
                        ie.Visible = true;
                        ie.Navigate(logger_uri, ref o, ref o, ref o, ref o);
                        return;
                    }
                }
                catch
                {
                    Console.WriteLine("Starting new instance of Internet Explorer.");
                }
            }

            Process.Start("iexplore", logger_uri);
        }

    }
}
