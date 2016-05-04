using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C2W_Client
{
    public class C2WAsyncCallbackEventArgs
    {
        public C2WAsyncCallbackEventArgs(C2WResponse response)
        {
            this.Response = response;
        }

        public C2WResponse Response { get; private set; }

        public bool Success
        {
            get { return Response.status == C2WResultValue.OK.ToString(); }
        }

        public string ResponseText { get { return Response.response; } }
    }
}
