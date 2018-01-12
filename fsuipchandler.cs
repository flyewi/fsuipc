using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using FSUIPC;


namespace fsuipcserve
{
  

  public class fsuipchandler : WebSocketBehavior
  {
        Fsuipcconnector fsuipcconnector;


    public fsuipchandler()
    {
        fsuipcconnector = new Fsuipcconnector();
    }

    void handleRequest (string name, MessageEventArgs e)
        {

            string json = "";

            //Console.WriteLine(!name.IsNullOrEmpty() ? String.Format("\"{0}\" to {1}", e.Data, name) : e.Data);
            Console.WriteLine("Name: {0}", name.IsNullOrEmpty() ? "-" : name);
            try {

                WS_Task ws_task =  JsonConvert.DeserializeObject<WS_Task>(e.Data);
                Console.WriteLine(ws_task.taskname);
                Console.WriteLine(JsonConvert.SerializeObject(ws_task));
                if (ws_task.taskname == null)
                {
                    return;
                }
                switch (ws_task.taskname.ToUpper())
                {
                    case "ADDOFFSET":
                        if (ws_task.offsetMessage != null)
                        {
                            fsuipcconnector.AddOffset(ws_task.offsetMessage);
                        } else
                        {
                            Errormessage.sendErrorMessage("ADDOFFSET", "Invalid ADD Object");
                        }

                        break;

                    case "GETPANELITEMS":
                        if (ws_task.panelname != null)
                        {
                            json = fsuipcconnector.getPanelState(ws_task.panelname);
                            Send(json);
                        }
                        else
                        {
                            Errormessage.sendErrorMessage("GETPANELITEMS", "NO Parameter Panelname");
                        }

                        break;
                    case "SETVALUE":
                        if (ws_task.offsetname != null && ws_task.value != null)
                        {
                            fsuipcconnector.setOffsetValue(ws_task.offsetname ,ws_task.value);
                            Send(json);
                        }
                        else
                        {
                            Errormessage.sendErrorMessage("SETVALUE", "OFFESTNAME and OFFSETVALUE REQUIRED");
                        }

                        break;
                    case "SETVALUEDIRECT":
                        if (ws_task.offsetname != null && ws_task.value != null)
                        {
                            try
                            {
                                fsuipcconnector.SendControlToFS(Convert.ToInt32(ws_task.offsetname), Convert.ToInt32(ws_task.value));

                            } catch(Exception ex)
                            {
                                Errormessage.sendErrorMessage("SETVALUE", "OFFSETNAME and OFFSETVALUE mut be type int");
                            }
                        }
                        else
                        {
                            Errormessage.sendErrorMessage("SETVALUE", "OFFSETNAME and OFFSETVALUE REQUIRED");
                        }

                        break;
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
