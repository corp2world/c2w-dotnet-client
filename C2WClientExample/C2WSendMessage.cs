using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using C2W_Client;

namespace C2WClientExample
{
    class C2WSendMessage
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Load config params");
            string userName = ConfigurationManager.AppSettings["username"];
            string password = ConfigurationManager.AppSettings["password"];
            string proxyServer = ConfigurationManager.AppSettings["proxyserver"];
            int? proxyPort = null;
            string proxyPortConf = ConfigurationManager.AppSettings["proxyport"];
            if (!string.IsNullOrEmpty(proxyPortConf))
            {
                try
                {
                    proxyPort = Convert.ToInt32(ConfigurationManager.AppSettings["proxyport"]);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error!!! Parameter proxy port setting is wrong.");
                }
                
            }

            string proxyUserName = ConfigurationManager.AppSettings["proxyusername"];
            string proxyPassword = ConfigurationManager.AppSettings["proxypassword"];

            Console.WriteLine("Create http service");
            IHttpService httpService = new HttpService();

            Console.WriteLine("Start with client params");

            httpService.Start(userName,password, proxyServer, proxyPort, proxyUserName, proxyPassword);

            Console.WriteLine("Create message");
            var message = new Message { topic = "topic message from .NET library", text = "text  message from .NET library" };

            Console.WriteLine("Add recipients");
            message.channelRecipients.Add(((int)EChannelType.PhoneSms).ToString(), new string[]{"+123456789191"});

            Console.WriteLine("Add response options");
            message.dialogOptions.Add("Accepted");
            message.dialogOptions.Add("Rejected");

            
            Console.WriteLine("Send message");
            var result = httpService.Send(message);
            if (result.status == ResultValue.OK.ToString())
                Console.WriteLine("Message sended sucessfully. Message Id = "+result.MessageId);
            else Console.WriteLine("Error - " + result.response);

            Console.WriteLine("Wait responses");
            Console.WriteLine("");
            var stopWhile = false;
            while (!stopWhile)
            {
                var responses = httpService.GetMessageResponses(result.MessageId);
                if (responses!= null && responses.Count() > 0)
                {
                    stopWhile = true;
                    foreach (var messageResponse in responses)
                    {
                        Console.WriteLine("Get responser : Message Id = " + messageResponse.messageId+
                            " user id = " + messageResponse.userId + " Responded option = " + messageResponse.respondedOption);
                    }
                    
                }
                Thread.Sleep(100); 
            }
            Console.WriteLine("");
            Console.WriteLine("Press any key to quit");
            Console.Read();
        }
    }
}
