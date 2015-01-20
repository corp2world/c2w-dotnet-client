using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2W_Client
{
    public interface IHttpService
    {
        void Start(string userName, string password, string proxyServer, int? proxyPort, string proxyUserName, string proxyPassword);
        /// <summary>
        /// Send message to Corp2World service
        /// </summary>
        /// <param name="message">message object to be sent</param>
        HttpResponse Send(Message message);

        /// <summary>
        /// Get message responses from Corp2World service
        /// </summary>
        /// <param name="messageId">message Id object to be sent</param>
        List<MessageResponse> GetMessageResponses(long messageId);
    }
}
