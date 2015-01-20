using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Web.Script.Serialization;

namespace C2W_Client
{
    [ClassInterface(ClassInterfaceType.None)]
    public class HttpService : IHttpService
    {
        private HttpWebRequest httpWebRequest = null;

        private string userName = String.Empty;
        private string password = String.Empty;
        private string proxyServer  = String.Empty;
        private int? proxyPort;
        private string proxyUserName;
        private string proxyPassword;

        public void Start(string userName, string password,
            string proxyServer, int? proxyPort, string proxyUserName, string proxyPassword)
        {
            httpWebRequest = GetRequest(Constans.SendMessageUrl, userName, password, proxyServer, proxyPort, proxyUserName, proxyPassword);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json; charset=utf-8";

            this.userName = userName;
            this.password = password;
            this.proxyServer = proxyServer;
            this.proxyPort = proxyPort;
            this.proxyUserName = proxyUserName;
            this.proxyPassword = proxyPassword;
        }

        public HttpResponse Send(Message message)
        {
            HttpResponse result = null;
            var writer = new StreamWriter(httpWebRequest.GetRequestStream());

            var js = new JavaScriptSerializer();

            string messageJs = js.Serialize(message);

            writer.Write(messageJs);
            writer.Close();

            try
            {
                var response = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseData = streamReader.ReadToEnd();
                    var jss = new JavaScriptSerializer();
                    result = jss.Deserialize<HttpResponse>(responseData);
                }

                response.Close();
            }
            catch (WebException ex)
            {
                if (result == null) result = new HttpResponse();
                result.status = "ERROR";
                result.response = "System error - " + ex.Message;
            }

            return result;
        }

        public List<MessageResponse> GetMessageResponses(long messageId)
        {
            var request = GetRequest(Constans.GetResponseUrl + messageId, userName, password, proxyServer, proxyPort, proxyUserName, proxyPassword);
            request.Method = "GET";
            List<MessageResponse> result = null;
            var response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseData = streamReader.ReadToEnd();
                var jss = new JavaScriptSerializer();
                result = jss.Deserialize<List<MessageResponse>>(responseData);
            }

            response.Close();

            return result;

        }


        protected HttpWebRequest GetRequest(string uri, string userName, string password,
            string proxyServer, int? proxyPort, string proxyUserName, string proxyPassword)
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            var dir = AssemblyDirectory;
            var certificate = X509Certificate.CreateFromCertFile(dir + "\\server.pfm");
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ClientCertificates.Add(certificate);
            request.Credentials = new NetworkCredential(userName, password);

            if (!string.IsNullOrEmpty(proxyServer) && proxyPort!=null)
            {
                var proxy = new WebProxy();
                proxy.Address = new Uri("http://"+proxyServer + ":" + proxyPort.ToString());
                
                if (!string.IsNullOrEmpty(proxyUserName))
                    proxy.Credentials = new NetworkCredential(proxyUserName, proxyPassword);
                request.Proxy=proxy;
            }
            return request;
        }

        protected string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

    }
}
