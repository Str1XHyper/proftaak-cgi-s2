using DAL.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.API
{
    public class APICalls
    {
        private CallsHandler callsHandler;
        public APICalls()
        {
            callsHandler = new CallsHandler();
        }
        public string APICall(string url) => callsHandler.CallUrl(url);
    }
}
