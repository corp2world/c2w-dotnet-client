using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2W_Client
{
    public static class Constans
    {
        public static string HttpResultPropertyMessageId = "messageId";
        public static string SendMessageUrl = "https://corp2world.com:9443/rest/message/post";
        public static string GetResponseUrl = "https://corp2world.com:9443/rest/message/response?messageId=";
    }
}
