using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DAL.API
{
    public class CallsHandler
    {
        public string CallUrl(string url)
        {
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string urlText = reader.ReadToEnd(); // it takes the response from your url. now you can use as your need  
            return urlText;
        }
    }
}
