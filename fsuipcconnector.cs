using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using FSUIPC;
using Newtonsoft.Json;
using WebSocketSharp;

namespace fsuipcserve
{

    class OffsetItem { 

        public OffsetItem (String type, String convert, Object ob)
        {
            this.type = type;
            this.convert = convert;
            this.offset = ob;
        }

        public string type { get; set; }
        public string convert { get; set; }
        public Object offset { get; set; }
    }

    class Fsuipcconnector
    {

        // Stores OffsetInformation for FSUIPCProcess 
        public Dictionary<String, OffsetItem> fuipcoffsets;
        // Stores all offsets that relate to a panel
        public Dictionary<String, String> fuipcpanels;
        public Dictionary<String, String> fuipcoffsetnames;

        private Offset<int> controlParam = new Offset<int>("sendControl", 0x3114, true);
        private Offset<int> sendControl = new Offset<int>("sendControl", 0x3110, true);


        private static Timer FSUIPCTimer;
        private static Boolean isConnectedtoFSUIPC = false;

        public Fsuipcconnector ()
        {
            fuipcoffsets = new Dictionary<String, OffsetItem>();          
            fuipcpanels = new Dictionary<String, String>();
            fuipcoffsetnames = new Dictionary<String, String>();
            SetFSUIPCTimer();

        }


        // Send Data to FSUIPC and get actual offsets
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
                    isConnectedtoFSUIPC = false;
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

        // Send Value directly to Offset
        public void SendControlToFS(int controlNumber, int parameterValue)
        {
            try
            {
                Console.WriteLine("Called Offset : {0} = {1}", controlNumber, parameterValue);
                this.sendControl.Value = controlNumber;
                this.controlParam.Value = parameterValue;
                FSUIPCProcess("sendControl");
            } catch (Exception ex)
            {
                Errormessage.sendErrorMessage("FSUIPC SendControlToFS", ex.Message);
            }

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
            string convert = o.convert.IsNullOrEmpty() ? "" : o.convert;

            // creates the right Offset Object and stores it in fuipcoffsets with key offset

            Object ob = new Object();

            switch (o.type.ToLower())
            {
                case "int":                    
                    fuipcoffsets.Add(offsetvalue, new OffsetItem( "int", convert,  new Offset<int>(o.offset)  ));
                    break;
                case "byte":
                    fuipcoffsets.Add(offsetvalue, new OffsetItem("byte", convert, new Offset<byte>(o.offset)));                    
                    break;
                case "bitarray":
                    fuipcoffsets.Add(offsetvalue, new OffsetItem("bitarray", convert, new Offset<System.Collections.BitArray>(o.offset,2)));                    
                    break;
                case "double":
                    fuipcoffsets.Add(offsetvalue, new OffsetItem("double", convert, new Offset<double>(o.offset)));                    
                    break;
                case "long":
                    fuipcoffsets.Add(offsetvalue, new OffsetItem("long", convert, new Offset<long>(o.offset)));               
                    break;
                case "short":
                    fuipcoffsets.Add(offsetvalue, new OffsetItem("short", convert, new Offset<short>(o.offset)));                 
                    break;
            }

            // Stores name of the offset as key and panelname
            fuipcpanels.Add(offsetname, panelname);
            // stores offsetname as key and offsetvalue to lookup in fuipcoffsets
            fuipcoffsetnames.Add(offsetname, offsetvalue);

        }

        public string getPanelState (string panelname)
        {
            ResponseList responselist = new ResponseList();
            string value = "";
            Object[] parameters;
            int viv;
            short vsv;
            byte vbv;
            long vlv;
            BitArray vbav;

            foreach (var offsetname in fuipcpanels)
            {
                if (offsetname.Value == panelname)
                {
                   
                    // offsetname zu panel ermitteln
                   if (fuipcoffsetnames.TryGetValue (offsetname.Key, out string  offsetval))
                    {
                        // Object zu offsetname ermitteln
                        if (fuipcoffsets.TryGetValue (offsetval, out OffsetItem op))
                        {

                            switch (op.type.ToLower())
                            {
                                case "int":
                                    viv = (op.offset as Offset<int>).Value;
                                    parameters = new Object[] { viv };
                                    break;
                                case "byte":
                                    vbv = (op.offset as Offset<byte>).Value;
                                    parameters = new Object[] { vbv };
                                    break;
                                case "long":
                                    vlv = (op.offset as Offset<long>).Value;
                                    parameters = new Object[] { vlv };
                                    break;
                                case "short":
                                    vsv = (op.offset as Offset<short>).Value;
                                    parameters = new Object[] { vsv };
                                    break;
                                case "bitarray":
                                    vbav = (op.offset as Offset<BitArray>).Value;
                                    parameters = new Object[] { vbav };
                                    break;
                                default:
                                    parameters = new Object[] { };
                                    break;

                            }

                            if (op.convert != null && op.convert.Length >2)
                            {
                                value = invokeConverter(op.convert, parameters);
                            }
                            else
                            {
                                value = parameters[0].ToString();
                            }

                            responselist.addToList(offsetname.Key, value);

                        }
                    }


                }
                   
            }

            return JsonConvert.SerializeObject(responselist.responses);
        }

        public void setOffsetValue(string offsetname, string value)
        {

            if (fuipcoffsets.TryGetValue(offsetname, out OffsetItem op))
            {
                try
                {
                    switch (op.type.ToLower())
                    {
                        case "int":
                            (op.offset as Offset<int>).Value = Convert.ToInt32(value);
                            break;
                        case "byte":
                            (op.offset as Offset<byte>).Value = Convert.ToByte(value);
                            break;
                        case "long":
                            (op.offset as Offset<long>).Value = Convert.ToInt64(value);
                            break;
                        case "short":
                            (op.offset as Offset<short>).Value = Convert.ToInt16(value);

                            break;
                        case "bitarray":
                            (op.offset as Offset<BitArray>).Value = JsonConvert.DeserializeObject<BitArray>(value);
                            break;
                        default:
                            Errormessage.sendErrorMessage("FSUIPC setOffsetValue", "Invalid Type of Offset found");
                            break;

                    }
                } catch(Exception ex)
                {
                    Errormessage.sendErrorMessage("FSUIPC setOffsetValue", "Error while setting value");
                }

            } else
            {
                Errormessage.sendErrorMessage("FSUIPC setOffsetValue", "Offsetvalue not found");
            }


        }
        public string invokeConverter(string converter, Object[] parameters)
        {
            Type t = typeof(fsxConverter);
            if (converter.Length > 2)
            {
                MethodInfo m = t.GetMethod(converter);
                if (m != null)
                {
                    return (String)m.Invoke(t, parameters).ToString();
                }
                else
                {
                    return parameters[0].ToString();
                }

            }
            else
            {
                return "";
            }
        }

        // Data ist collected every 200ms from FSUIPC
        // If Connection is not open, a Connect Request will be tried.
        private static void SetFSUIPCTimer()
        {
            
            FSUIPCTimer = new System.Timers.Timer(200);
            
            FSUIPCTimer.Elapsed += OnTimedEvent;
            FSUIPCTimer.AutoReset = true;
            FSUIPCTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (! isConnectedtoFSUIPC)
            {
                try
                {

                    Console.Write(".");
                    FSUIPCConnection.Open();
                     isConnectedtoFSUIPC = true;

                } catch (Exception ex)
                {
                    isConnectedtoFSUIPC = false;
                }
            } else
            {
                try
                {
                    FSUIPCConnection.Process();

                } catch (Exception ex)
                {
                    Errormessage.sendErrorMessage("FSUIPC Process", ex.Message);
                    FSUIPCConnection.Close();
                    isConnectedtoFSUIPC = false;
                }
            }

            
        }

    }
}
