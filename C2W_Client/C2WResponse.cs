using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C2W_Client
{
    public class C2WResponse
    {
        public string status { get; set; }
        public string response { get; set; }
        public Dictionary<String, String> properties = new Dictionary<String, String>();

        public long MessageId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(properties[C2WConstans.HttpResultPropertyMessageId]);
                }
                catch (Exception)
                {
                    return -1;
                }

            }
        }

    }

    public enum C2WResultValue { OK, ERROR }
}
