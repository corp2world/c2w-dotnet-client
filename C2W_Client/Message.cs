using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2W_Client
{
    /**
     * Class represent Message object.
     * Message has topic, text (main message content) and additional properties and recipients.
     */

    public class Message
    {
        public string topic { get; set; }
        public string text { get; set; }
        public Int64 timestamp { get; set; }
        public bool test { get; set; }
        public Dictionary<String, String> properties = new Dictionary<String, String>();
        public Dictionary<String, String[]> channelRecipients = new Dictionary<String, String[]>();
        public List<String> dialogOptions = new List<String>();

        public Message()
        {
            timestamp = ConvertToUnixTimestamp(DateTime.Now.ToUniversalTime());
            test = false;
        }

        protected Int64 ConvertToUnixTimestamp(DateTime date)
        {
            return (Int64)(date.ToLocalTime() - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;
        }

    }
}
