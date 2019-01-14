using HttpClientProxy.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy.Handler
{
    public interface IHttpClientHandler
    {
        object Request(HttpProxyRequestContext context);
    }
}
