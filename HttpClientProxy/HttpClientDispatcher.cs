using HttpClientProxy.Context;
using HttpClientProxy.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy
{
    public class HttpClientDispatcher
    {
        public object Request(HttpProxyContext proxyContext, object[] values, object client)
        {
            HttpProxyRequestContext context = GenerateContext(proxyContext, values, client);

            for (int i = 0; i < proxyContext.Filters.Count; i++)
            {
                var filter = proxyContext.Filters[i];
                filter.Before();
            }
            var result = context.Handler.Request(context);

            for (int i = proxyContext.Filters.Count - 1; i >= 0; i--)
            {
                var filter = proxyContext.Filters[i];
                filter.After();
            }

            if (proxyContext.MethodInfo.ReturnType == typeof(void))
            {
                return null;
            }
            return result;

        }

        private HttpProxyRequestContext GenerateContext(HttpProxyContext proxyContext, object[] values, object client)
        {
            return new HttpProxyRequestContext.Builder(proxyContext, values, client).Build();
        }


    }
}
