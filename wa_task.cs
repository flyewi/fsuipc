// Objekt, das die Nachrichten aus dem Websocket empfängt

public class WS_Task
{
    // valid Tasks:
    // {"taskname" : "CONNECT"                }   : Connect to FSUIPC
    // {"taskname" : "PROCESS"                } : Process FSUIPC Update
    // {"taskname" : "GETPANEL  "panelname": "mypanel" }
    // {"taskname" : "ADDOFFSET  "offset": "mypanel", offsettype, convert }
    // {"taskname" : "SETOFFSET  "offset"  value  }

    public string taskname { get; set; }

    public string offset { get; set; }

}