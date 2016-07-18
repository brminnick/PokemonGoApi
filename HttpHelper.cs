using System.Net;
using System.Collections.Generic;

namespace PokemonGoApi
{
    public static class HttpHelper
    {
        static CookieContainer cookieContainer = null;
        public static string PostString(string uri, byte [] data = null, List<KeyValuePair<string, string>> headers = null, string returnHeader = "")
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "POST";

            if (cookieContainer == null)
                cookieContainer = new CookieContainer();

            request.CookieContainer = cookieContainer;

            if (data != null)
                request.ContentLength = data.Length;
            else
                request.ContentLength = 0;

            request.ServicePoint.Expect100Continue = false;
            request.KeepAlive = true;

            request.AllowAutoRedirect = false;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    if (header.Key == "User-Agent")
                        request.UserAgent = header.Value;
                    else if (header.Key == "Content-Type")
                        request.ContentType = header.Value;
                    else
                        request.Headers[header.Key] = header.Value;
                }
            }
            request.KeepAlive = true;
            
            request.Accept = "*/*";

            var stream = request.GetRequestStream();

            if (data != null && data.Length > 0)
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            if (returnHeader != "")
                return response.Headers[returnHeader];
            
            return new System.IO.StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        public static byte [] Post(string uri, byte[] data = null, List<KeyValuePair<string, string>> headers = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "POST";

            if (cookieContainer == null)
                cookieContainer = new CookieContainer();

            request.CookieContainer = cookieContainer;

            if (data != null)
                request.ContentLength = data.Length;
            else
                request.ContentLength = 0;

            request.ServicePoint.Expect100Continue = false;
            request.KeepAlive = true;

            request.AllowAutoRedirect = false;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    if (header.Key == "User-Agent")
                        request.UserAgent = header.Value;
                    else if (header.Key == "Content-Type")
                        request.ContentType = header.Value;
                    else
                        request.Headers[header.Key] = header.Value;
                }
            }
            request.KeepAlive = true;

            request.Accept = "*/*";

            var stream = request.GetRequestStream();

            if (data != null && data.Length > 0)
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            return new System.IO.BinaryReader(response.GetResponseStream()).ReadBytes((int)response.ContentLength);
        }
        public static byte [] parseQuery(List<KeyValuePair<string, string>> pairs)
        {
            bool isFirst = true;
            string query = "";
            foreach (KeyValuePair<string, string> pair in pairs)
            {
                if (isFirst)
                    query = pair.Key + "=" + pair.Value;
                else 
                    query += "&" + pair.Key + "=" + pair.Value;
                isFirst = false;
            }
            return System.Text.Encoding.Default.GetBytes(query);
        }
    }
}
