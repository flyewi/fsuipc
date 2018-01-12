// Objekt, das die Nachrichten aus dem Websocket empfängt
namespace fsuipcserve
{


    public class WS_Task
    {
        // valid Tasks:
        // {"taskname" : "CONNECT"                }   : Connect to FSUIPC
        // {"taskname" : "PROCESS"                } : Process FSUIPC Update
        // {"taskname" : "GETPANEL  "panelname": "mypanel" }
        // {"taskname" : "ADDOFFSET  "offset": "mypanel", offsettype, convert }
        // {"taskname" : "SETOFFSET  "offset"  value  }

        public string taskname { get; set; }
        public OffsetMessage offsetMessage { get; set; }
        public string panelname { get; set; }
        public string offset { get; set; }
        public string offsetname { get; set; }
        public string value { get; set; }

    }

}