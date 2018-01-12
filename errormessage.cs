using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fsuipcserve
{

    class MsgObject
    {

        public  MsgObject(string msgtype, string msg)
        {
            this.msgtype = msgtype;
            this.msg = msg;
        }
        public string msgtype   { get; set; }
        public string msg       { get; set; }

    }

    class Errormessage
    {

        public static void sendErrorMessage(string msgtype, string msg)
        {
            string mob = JsonConvert.SerializeObject(new MsgObject(msgtype, msg));

        }



    }
}
