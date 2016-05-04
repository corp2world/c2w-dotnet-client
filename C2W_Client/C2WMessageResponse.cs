using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C2W_Client
{
    public class C2WMessageResponse
    {
        public Dictionary<string, string> properties = new Dictionary<string, string>();

        public long messageId { get; set; }

        public long timestamp { get; set; }

        public String respondedOption { get; set; }

        public String userId { get; set; }

        public int channelId { get; set; }
    }
}
