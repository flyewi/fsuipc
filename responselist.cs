using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fsuipcserve
{
    public class ResponseList
    {
        public Dictionary<String, String> responses;


        public void addToList(String name, String value)
        {

            responses.Add(name, value);
        }

        public ResponseList()
        {
            responses = new Dictionary<String, String>();
        }

    }

    public class ResponseItem
    {
        public string type { get; set; }
        public string value { get; set; }

        public ResponseItem(String type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
}
