using HttpClientProxy.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpClientProxy
{
    public class SimpleUrl
    {
        public string Protocol { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Path
        {

            get
            {

                StringBuilder result = new StringBuilder();
                foreach (var p in Paths)
                {
                    result.Append('/').Append(p);
                }
                return result.ToString();
            }
        }
        public string QueryString
        {
            get
            {
                StringBuilder result = new StringBuilder();
                foreach (var kv in Parameters)
                {
                    result.Append(kv.Key).Append('=').Append(kv.Value).Append('&');
                }
                if (result.Length > 0)
                {
                    result.Remove(result.Length - 1, 1);
                }
                return result.ToString();
            }
        }

        public Dictionary<string, string> Parameters { private get; set; }

        public List<string> Paths { get; private set; }


        public SimpleUrl(string url)
        {
            Parameters = new Dictionary<string, string>();
            Paths = new List<string>();
            ResolveProtocol(url);
        }

        private SimpleUrl()
        {
        }


        private void ResolveProtocol(string url)
        {
            int i = 0;
            StringBuilder protocol = new StringBuilder(5);
            for (; i < url.Length; i++)
            {
                char c = url[i];

                if (c == ':')
                {
                    break;
                }
                protocol.Append(c);
            }
            Protocol = protocol.ToString();
            if (i < url.Length)
            {
                ResolveHost(url, i + 3);
            }

        }
        private void ResolveHost(string url, int startIndex)
        {
            StringBuilder host = new StringBuilder();
            for (; startIndex < url.Length; startIndex++)
            {
                char c = url[startIndex];
                if (c == ':' || c == '/' || c == '?' || c == '#')
                {
                    Host = host.ToString();
                    switch (c)
                    {
                        case ':':
                            ResolvePort(url, startIndex + 1);
                            return;
                        case '/':
                            ResolvePath(url, startIndex + 1);
                            return;
                        case '?':
                            ResolveQueryString(url, startIndex + 1);
                            return;
                        default:
                            return;
                    }
                }
                else
                {
                    host.Append(c);
                }
            }
            Host = host.ToString();
        }
        private void ResolvePort(string url, int startIndex)
        {
            StringBuilder strPort = new StringBuilder();
            int port = 0;
            for (; startIndex < url.Length; startIndex++)
            {
                char c = url[startIndex];
                if (c == '/' || c == '?' || c == '#')
                {
                    int.TryParse(strPort.ToString(), out port);
                    Port = port;
                    switch (c)
                    {
                        case '/':
                            ResolvePath(url, startIndex + 1);
                            return;
                        case '?':
                            ResolveQueryString(url, startIndex + 1);
                            return;
                        default:
                            return;
                    }
                }
                else
                {
                    strPort.Append(c);
                }
            }

            int.TryParse(strPort.ToString(), out port);
            Port = port;
        }
        private void ResolvePath(string url, int startIndex)
        {

            StringBuilder p = new StringBuilder(20);
            StringBuilder path = new StringBuilder();
            for (; startIndex < url.Length; startIndex++)
            {
                char c = url[startIndex];

                if (c == '/')
                {
                    Paths.Add(p.ToString());
                    p.Clear();
                }
                else
                {
                    p.Append(c);
                }

                if (c == '?' || c == '#')
                {
                    if (p.Length > 0)
                    {
                        p.Remove(p.Length - 1, 1);
                    }
                    Paths.Add(p.ToString());
                    switch (c)
                    {
                        case '?':
                            ResolveQueryString(url, startIndex + 1);
                            return;
                        default:
                            return;
                    }
                }
                else
                {
                    path.Append(c);
                }
            }

            Paths.Add(p.ToString());
        }


        private void ResolveQueryString(string url, int startIndex)
        {

            StringBuilder queryString = new StringBuilder();
            //Dictionary<string, string> parameters = new Dictionary<string, string>();
            StringBuilder currKey = new StringBuilder();
            StringBuilder currValue = new StringBuilder();
            StringBuilder curr = currKey;
            for (; startIndex < url.Length; startIndex++)
            {
                char c = url[startIndex];
                if (c == '#')
                {
                    break;
                }
                else
                {
                    if (c == '&')
                    {
                        if (currKey.Length > 0 && currValue.Length > 0)
                        {
                            Parameters[currKey.ToString()] = currValue.ToString();
                        }
                        currKey.Clear();
                        currValue.Clear();
                        curr = currKey;
                    }

                    else if (c == '=')
                    {
                        curr = currValue;
                    }
                    else
                    {
                        curr.Append(c);
                    }
                    queryString.Append(c);
                }
            }
            if (currKey.Length > 0 && currValue.Length > 0)
            {
                Parameters[currKey.ToString()] = currValue.ToString();
            }
        }

        public string ToUrl()
        {
            StringBuilder url = new StringBuilder();
            url.Append(Protocol).Append("://").Append(Host);
            if (Port > 0)
            {
                /*
                if ((Port == 80 && "http".Equals(Host, StringComparison.OrdinalIgnoreCase)==false)
                    || (Port == 443 && "https".Equals(Host, StringComparison.OrdinalIgnoreCase) == false))
                {
                }*/
                url.Append(':').Append(Port);
            }

            url.Append(Path);
            if (Parameters.Count > 0)
            {
                url.Append('?');
                foreach (var kv in Parameters)
                {
                    url.Append(kv.Key).Append('=').Append(kv.Value).Append('&');
                }
                url.Remove(url.Length - 1, 1);
            }
            return url.ToString();
        }

        /******************************************************/
        #region SetParameter

        public SimpleUrl SetParameter(IDictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return this;
            }
            foreach (var kv in parameters)
            {
                Parameters[kv.Key] = kv.Value;
            }
            return this;
        }
        public SimpleUrl SetParameter(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return this;
            }
            if (value == null)
            {
                Parameters.Add(key, null);
            }
            Type type = value.GetType();

            if (TypeUtils.IsPrimitive(value.GetType()))
            {
                Parameters.Add(key, value.ToString());
            }
            else
            {
                Parameters.Add(key, HttpUtility.UrlEncode(value.ToString()));//urlencode
            }
            return this;
        }
        public SimpleUrl SetParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key) == false || string.IsNullOrEmpty(value) == false)
            {
                Parameters[key] = HttpUtility.UrlEncode(value);//urlencode
            }
            return this;
        }
        public SimpleUrl SetParameter(string key, int value)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                Parameters[key] = value.ToString();
            }
            return this;
        }
        public SimpleUrl SetParameter(string key, long value)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                Parameters[key] = value.ToString();
            }
            return this;
        }
        public SimpleUrl SetParameter(string key, bool value)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                Parameters[key] = value.ToString();
            }
            return this;
        }
        public SimpleUrl RemoveParameter(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                Parameters.Remove(key);
            }
            return this;
        }
        public SimpleUrl CleaParameters()
        {
            this.Parameters.Clear();
            return this;
        }
        public string GetParameter(string key)
        {
            string result = null;
            if (Parameters.TryGetValue(key, out result))
            {
                if (string.IsNullOrEmpty(result))
                {
                    result = HttpUtility.UrlDecode(result);
                }
            }
            return result;
        }
        #endregion
        public SimpleUrl AppendPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return this;
            }
            path = path.TrimStart('/');
            if (path.Length > 0)
            {
                var lpath = Paths.Last();
                if (lpath == "")
                {
                    Paths.Remove(lpath);
                }
            }
            StringBuilder p = new StringBuilder(path.Length);

            for (int i = 0; i < path.Length; i++)
            {
                var c = path[i];
                if (c == '/')
                {
                    Paths.Add(p.ToString());
                    p.Clear();
                }
                else
                {
                    p.Append(c);
                }
            }
            Paths.Add(p.ToString());
            return this;
        }


        public SimpleUrl SetPathParameter(string key, string value)
        {
            string val = HttpUtility.UrlEncode(value);
            for (int i = 0; i < Paths.Count; i++)
            {
                if (string.Equals('{' + key + '}', Paths[i]))
                {
                    Paths[i] = value;
                }
            }

            return this;
        }

        public SimpleUrl SetPathParameter(string key, object value)
        {
            string val = null;
            if (value != null)
            {
                val = HttpUtility.UrlEncode(value.ToString());
            }
            for (int i = 0; i < Paths.Count; i++)
            {
                if (string.Equals('{' + key + '}', Paths[i]))
                {
                    Paths[i] = HttpUtility.UrlEncode(val);
                }
            }

            return this;
        }




        public override string ToString()
        {
            return this.ToUrl();
        }

        public SimpleUrl Clone()
        {
            SimpleUrl result = new SimpleUrl()
            {
                Protocol = this.Protocol,
                Host = this.Host,
                Port = Port,
                Paths = new List<string>(),
                Parameters = new Dictionary<string, string>()
            };

            foreach (var p in Paths)
            {
                result.Paths.Add(p);
            }


            foreach (var p in Parameters)
            {
                result.Parameters[p.Key] = p.Value;
            }

            return result;
        }

        interface IPath
        {
            //string 
        }
    }
}
