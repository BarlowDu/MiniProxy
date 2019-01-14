using HttpClientProxy.Enum;
using HttpClientProxy.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy.Context
{
    public class HttpProxyContext
    {
        //Accept,ContentType,Filters,HttpMethod,MethodInfo,ParameterInfos,TimeOut,Url
        public string Accept { get; private set; } = "application/json";
        public string ContentType { get; private set; } = "application/x-www-form-urlencoded";
        public List<ProxyFilterAttribute> Filters { get; private set; }
        public HttpMethod HttpMethod { get; private set; } = HttpMethod.GET;

        public MethodInfo MethodInfo;

        public IList<HttpArgument> Arguments;

        public int TimeOut { get; private set; } = -1;
        public SimpleUrl Url { get; private set; }




        public class Builder
        {
            private HttpProxyContext context;

            public Builder()
            {
                context = new HttpProxyContext()
                {
                    HttpMethod = HttpMethod.GET,
                    ContentType = "application/json",
                    TimeOut = -1,
                    Filters = new List<ProxyFilterAttribute>()
                };
            }

            public Builder SetAccept(string accept)
            {
                context.Accept = accept;
                return this;
            }
            public Builder SetContentType(string contentType)
            {
                context.ContentType = contentType;
                return this;
            }
            public Builder AddFilter(ProxyFilterAttribute filter)
            {
                context.Filters.Add(filter);
                return this;
            }

            public Builder AddFilters(IEnumerable<ProxyFilterAttribute> filters)
            {
                if (filters!= null)
                {
                    context.Filters.AddRange(filters);
                }
                return this;
            }
            public Builder SetHttpMethod(HttpMethod httpMethod)
            {
                context.HttpMethod = httpMethod;
                return this;
            }
            public Builder SetMethodInfo(MethodInfo method)
            {
                context.MethodInfo = method;
                return this;
            }

            public Builder SetArguments(IList<HttpArgument> arguments)
            {

                context.Arguments = arguments;
                return this;
            }
            public Builder SetTimeOut(int timeout)
            {
                context.TimeOut = timeout;
                return this;
            }
            public Builder SetUrl(SimpleUrl url)
            {
                context.Url = url;
                return this;
            }




            public HttpProxyContext Build()
            {
                return context;
            }
        }
    }
}
