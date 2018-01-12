using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using FSUIPC;


namespace fsuipcserve
{
  

  public class fsuipchandler : WebSocketBehavior
  {

    void handleRequest (string name, MessageEventArgs e)
        {
            //Console.WriteLine(!name.IsNullOrEmpty() ? String.Format("\"{0}\" to {1}", e.Data, name) : e.Data);
            Console.WriteLine("Name: {0}", name.IsNullOrEmpty() ? "-" : name);
            try {

                WS_Task ws_task =  JsonConvert.DeserializeObject<WS_Task>(e.Data);
                Console.WriteLine(ws_task.taskname);
                Console.WriteLine(JsonConvert.SerializeObject(ws_task));

                switch (ws_task.taskname.ToUpper())
                {

                    case "CONNECT":
                        try
                        {
                            FSUIPCConnection.Open();
                            Console.WriteLine("Connected to {0}", FSUIPCConnection.FlightSimVersionConnected);

                           
                        }
                        catch (FSUIPCException ex)
                        {
                           
                        }

                        break;


                }


            }
            catch ( Exception ex){
                Console.WriteLine(ex.ToString());
            }
        }

    protected override void OnMessage (MessageEventArgs e)
    {
      var name = Context.QueryString["name"];
      handleRequest(name, e);
    }
  }
}
