using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C2W_Client
{
    public static class C2WConstans
    {
        public static string HttpResultPropertyMessageId = "messageId";
        public static string SendMessageUrl = "https://corp2world.com:9443/rest/message/post";
        public static string GetResponseUrl = "https://corp2world.com:9443/rest/message/response?messageId=";
    }
}
