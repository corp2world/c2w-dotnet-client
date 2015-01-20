using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace C2W_Client
{
    public class HttpResponse
    {
        public string status { get; set; }
        public string response { get; set; }
        public Dictionary<String, String> properties = new Dictionary<String, String>();


        [ScriptIgnore]
        public long MessageId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(properties[Constans.HttpResultPropertyMessageId]);
                }
                catch (Exception)
                {
                    return -1;
                }

            }
        }

    }

    public enum ResultValue {OK, ERROR}
}
