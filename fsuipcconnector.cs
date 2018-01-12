using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSUIPC;
using WebSocketSharp;

namespace fsuipcserve
{
    class fsuipcconnector
    {

        // Stores OffsetInformation for FSUIPCProcess 
        public Dictionary<String, Object> fuipcoffsets;
        // Stores all offsets that relate to a panel
        public Dictionary<String, String> fuipcpanels;
        public Dictionary<String, String> fuipcoffsetnames;

        private Offset<int> controlParam = new Offset<int>("sendControl", 0x3114, true);
        private Offset<int> sendControl = new Offset<int>("sendControl", 0x3110, true);


        public void FSUIPCProcess(string s = "")
        {
            try
            {
                FSUIPCConnection.Process(s);

            }
            catch (FSUIPCException ex)
            {
                if (ex.FSUIPCErrorCode == FSUIPCError.FSUIPC_ERR_SENDMSG)
                {                    
                    FSUIPCConnection.Close();
                    Console.WriteLine("The connection to Flight Sim has been lost.");
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception)
            {
            }
        }

        public void SendControlToFS(int controlNumber, int parameterValue)
        {

            Console.WriteLine("Called Offset : {0} = {1}", controlNumber, parameterValue);
            this.sendControl.Value = controlNumber;
            this.controlParam.Value = parameterValue;
            FSUIPCProcess("sendControl");

        }



        // Stores a OffsetMessage in the fsuipc Offset List.
        public void AddOffset (OffsetMessage o)
        {

            string offsetvalue = "";
            try
            {
                if (o.offset > 0)
                {
                    offsetvalue = o.offset.ToString();
                }
                else
                {
                    // invalid offset
                }
            }
            catch (Exception ex)
            {
                // invalid offset
            }



            string offsetname = o.offsetname.IsNullOrEmpty() ? o.offset.ToString() : o.offsetname;
            string panelname = o.panel.IsNullOrEmpty() ? "GLOBAL" : o.panel;


            // creates the right Offset Object and stores it in fuipcoffsets with key offset
            switch (o.type.ToLower())
            {
                case "int":
                    fuipcoffsets.Add(offsetvalue, new Offset<int>(o.offset));
                    break;
                case "byte":
                    fuipcoffsets.Add(offsetvalue, new Offset<byte>(o.offset));
                    break;
                case "bitarray":
                    fuipcoffsets.Add(offsetvalue, new Offset<System.Collections.BitArray>(o.offset, 2));
                    break;
                case "double":
                    fuipcoffsets.Add(offsetvalue, new Offset<double>(o.offset));
                    break;
                case "long":
                    fuipcoffsets.Add(offsetvalue, new Offset<long>(o.offset));
                    break;
                case "short":
                    fuipcoffsets.Add(offsetvalue, new Offset<short>(o.offset));
                    break;
            }

            // Stores name of the offset as key and panelname
            fuipcpanels.Add(offsetname, panelname);
            // stores offsetname as key and offsetvalue to lookup in fuipcoffsets
            fuipcoffsetnames.Add(offsetname, offsetvalue);

        }
    }
}
