using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace C2W_Client
{
    public class C2WService 
    {
        public event EventHandler<C2WAsyncCallbackEventArgs> AsyncCallback;

        private HttpWebRequest httpWebRequest = null;

        private string userName = String.Empty;
        private string password = String.Empty;
        private string proxyServer = String.Empty;
        private int? proxyPort;
        private string proxyUserName;
        private string proxyPassword;

        private C2WMessage message;

        /// <summary>
        /// Send message to Corp2World service
        /// </summary>
        /// <param name="message">message object to be sent</param>
        public void Start(string userName, string password,
            string proxyServer, int? proxyPort, string proxyUserName, string proxyPassword)
        {
            httpWebRequest = GetRequest(C2WConstans.SendMessageUrl, userName, password, proxyServer, proxyPort, proxyUserName, proxyPassword);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json; charset=utf-8";

            this.userName = userName;
            this.password = password;
            this.proxyServer = proxyServer;
            this.proxyPort = proxyPort;
            this.proxyUserName = proxyUserName;
            this.proxyPassword = proxyPassword;
        }

        /// <summary>
        /// Send message to Corp2World service asynchronously
        /// </summary>
        /// <param name="message">message object to be sent</param>
        public C2WResponse Send(C2WMessage message)
        {
            this.message = message;
            C2WResponse result = null;

            var task = httpWebRequest.GetRequestStreamAsync();
            using (var streamWriter = new StreamWriter(task.Result))
            {
                var postData = JsonConvert.SerializeObject(this.message).ToString();
                streamWriter.Write(postData);
            }

            try
            {
                var respTask = httpWebRequest.GetResponseAsync();
                var response = (HttpWebResponse)respTask.Result;
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseData = streamReader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<C2WResponse>(responseData);
                }
            }
            catch (WebException ex)
            {
                if (result == null) result = new C2WResponse();
                result.status = "ERROR";
                result.response = "System error - " + ex.Message;
            }

            return result;
        }

        public void SendAsync(C2WMessage message)
        {
            this.message = message;
            httpWebRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), httpWebRequest);
        }

        /// <summary>
        /// Get message responses from Corp2World service
        /// </summary>
        /// <param name="messageId">message Id object to be sent</param>
        public List<C2WMessageResponse> GetMessageResponses(long messageId)
        {
            var request = GetRequest(C2WConstans.GetResponseUrl + messageId, userName, password, proxyServer, proxyPort, proxyUserName, proxyPassword);
            request.Method = "GET";
            List<C2WMessageResponse> result = null;
            var respTask = request.GetResponseAsync();
            var response = (HttpWebResponse)respTask.Result;
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseData = streamReader.ReadToEnd();
                result = JsonConvert.DeserializeObject<List<C2WMessageResponse>>(responseData);
            }
            return result;
        }

        protected HttpWebRequest GetRequest(string uri, string userName, string password,
            string proxyServer, int? proxyPort, string proxyUserName, string proxyPassword)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Credentials = new NetworkCredential(userName, password);

            /*if (!string.IsNullOrEmpty(proxyServer) && proxyPort != null)
            {
                var proxy = new WebProxy();
                proxy.Address = new Uri("http://" + proxyServer + ":" + proxyPort.ToString());

                if (!string.IsNullOrEmpty(proxyUserName))
                    proxy.Credentials = new NetworkCredential(proxyUserName, proxyPassword);
                request.Proxy = proxy;
            }*/
            return request;
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the stream request operation

            var postStream = request.EndGetRequestStream(asynchronousResult);

            // Create the post data
            var postData = JsonConvert.SerializeObject(this.message).ToString();
            var byteArray = Encoding.UTF8.GetBytes(postData);
            postStream.Write(byteArray, 0, byteArray.Length);
            //Start the web request
            request.BeginGetResponse(new AsyncCallback(GetResponceStreamCallback), request);
        }

        private void GetResponceStreamCallback(IAsyncResult callbackResult)
        {
            var request = (HttpWebRequest)callbackResult.AsyncState;
            var response = (HttpWebResponse)request.EndGetResponse(callbackResult);
            using (StreamReader httpWebStreamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = JsonConvert.DeserializeObject<C2WResponse>(httpWebStreamReader.ReadToEnd());
                OnDownloadGameEdit(result);
            }

        }

        private void OnDownloadGameEdit(C2WResponse response)
        {
            var handler = this.AsyncCallback;
            if (handler != null)
                handler(this, new C2WAsyncCallbackEventArgs(response));
        }

    }

}
