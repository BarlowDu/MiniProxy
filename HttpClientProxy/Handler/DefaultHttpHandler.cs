using HttpClientProxy.Context;
using HttpClientProxy.Enum;
using HttpClientProxy.Handler;
using HttpClientProxy.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace HttpClientProxy.Handler
{
    public class DefaultHttpHandler : IHttpClientHandler
    {

        public HttpResponseMessage Get(HttpProxyRequestContext context)
        {
            HttpClient client = new HttpClient();

            SimpleUrl urlComponent = context.Url;
            foreach (var arg in context.Arguments)
            {
                if (arg.ArgType == HttpArgType.QueryString)
                {
                    urlComponent.SetParameter(arg.HttpArgName, arg.Value);
                }
            }
            string url = urlComponent.ToUrl();
            return client.GetAsync(url).Result;
        }

        public HttpResponseMessage Post(HttpProxyRequestContext context)
        {
            HttpClient client = new HttpClient();
            SimpleUrl urlComponent = context.Url;
            foreach (var arg in context.Arguments)
            {
                if (arg.ArgType == HttpArgType.QueryString)
                {
                    urlComponent.SetParameter(arg.HttpArgName, arg.Value);
                }
            }
            string url = urlComponent.ToUrl();
            ///TODO body
            return client.GetAsync(url).Result;
        }

        public object Request(HttpProxyRequestContext context)
        {
            HttpResponseMessage responseMessage;
            if (context.HttpMethod == Enum.HttpMethod.POST)
            {
                responseMessage = Post(context);
            }
            else
            {
                responseMessage = Get(context);
            }
            if (context.MethodInfo.ReturnType == typeof(HttpResponseMessage))
            {
                return responseMessage;
            }
            if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception();
            }
            if (TypeUtils.IsModel(typeof(string)) == false)
            {
                return responseMessage.Content.ReadAsStringAsync().Result;
            }

            return null;
        }
    }
}
