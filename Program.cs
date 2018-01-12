using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using FSUIPC;

namespace fsuipcserve
{
  public class Program
  {
    public static void Main (string[] args)
    {
      
      
      var httpsv = new HttpServer (System.Net.IPAddress.Any, 4649);

      httpsv.DocumentRootPath = ConfigurationManager.AppSettings["DocumentRootPath"];

      // Set the HTTP GET request event.
      httpsv.OnGet += (sender, e) => {
          var req = e.Request;
          var res = e.Response;

          var path = req.RawUrl;
          if (path == "/")
            path += "index.html";

          byte[] contents;
          if (!e.TryReadFile (path, out contents)) {
            res.StatusCode = (int) HttpStatusCode.NotFound;
            return;
          }

          if (path.EndsWith (".html")) {
            res.ContentType = "text/html";
            res.ContentEncoding = Encoding.UTF8;
          }
          else if (path.EndsWith (".js")) {
            res.ContentType = "application/javascript";
            res.ContentEncoding = Encoding.UTF8;
          }

          res.WriteContent (contents);
        };

      // Add the WebSocket services.
      httpsv.AddWebSocketService<fsuipchandler> ("/Echo");
      
      httpsv.Start ();
      if (httpsv.IsListening) {
                Console.WriteLine("=====================================================================================");
                Console.WriteLine ("FSUIPC Server: ws://{0}:{1}", System.Net.IPAddress.Any, httpsv.Port);
                Console.WriteLine("=====================================================================================");
                Console.WriteLine("Services:");
                foreach (var path in httpsv.WebSocketServices.Paths)
                        Console.WriteLine ("- {0}", path);
                }

      Console.WriteLine ("\nPress Enter key to stop the server...");
      Console.ReadLine ();

      httpsv.Stop ();
    }
  }
}
