using HttpClientProxy.Attr;
using HttpClientProxy.Enum;
using HttpClientProxy.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy.Context
{
    public class HttpProxyRequestContext
    {
        public string Accept { get; private set; }
        public List<HttpArgument> Arguments { get; set; }
        public object Client { get; private set; }
        public string ContentType { get; private set; }
        public IHttpClientHandler Handler { get; private set; }
        public HttpMethod HttpMethod { get; private set; }
        public MethodInfo MethodInfo { get; set; }

        public int TimeOut { get; private set; }
        public SimpleUrl Url { get; private set; }






        private HttpProxyRequestContext(HttpProxyContext proxyContext, object[] values, object client)
        {
            MethodInfo = proxyContext.MethodInfo;

            ///TODO:Clone
            Url = proxyContext.Url.Clone();
            HttpMethod = proxyContext.HttpMethod;
            ContentType = proxyContext.ContentType;
            Accept = proxyContext.Accept;
            TimeOut = proxyContext.TimeOut;
            Client = client;

            this.Handler = new DefaultHttpHandler();

            Arguments = new List<HttpArgument>();
            for (int i = 0; i < values.Length; i++)
            {
                Arguments.Add(new HttpArgument(proxyContext.Arguments[i], values[i]));
            }


        }

        public void SetHandler(IHttpClientHandler handler)
        {
            this.Handler = handler;
        }

        public class Builder
        {
            HttpProxyRequestContext context;
            public Builder(HttpProxyContext proxyContext, object[] values, object client)
            {
                context = new HttpProxyRequestContext(proxyContext, values, client);
            }



            public HttpProxyRequestContext Build()
            {
                return context;
            }
        }

    }
}
