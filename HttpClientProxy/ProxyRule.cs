using HttpClientProxy.Attr;
using HttpClientProxy.Context;
using HttpClientProxy.Enum;
using HttpClientProxy.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy
{
    /* 基本原则:
     * 1.参数传递方式:path,querystring,body
     * 2.不支持ref,out,params等关键字
     * 3.只支持GET,POST两种情况
     * 4.只支持application/x-www-form-urlencoded,application/json两种ContentType
     * 5.检查URL是否合法
     * 4.接口必须有HttpProxyAttribute注解,(--方法必须有RequestMapping注解--)
     * 
     * 
     * 参数位置:
     * 1.GET
     * 1.1 只支持path,querystring两种方式,如果参数有被标注为body时直接忽略
     * 2. POST
     * 2.1 三种传参方式都支持
     * 2.2 如果ContentType=application/x-www-form-urlencoded,可以传递多个body参数
     * 2.2.1 一个body参数且为实体:将按实体属性进行拼接   #(如果用反射是否要考虑性能) 
     * 2.2.2 除2.2.1的其他情况:将各参数进行拼接
     * 2.3 如果ContentType=appllication/json,只可以传递一个参数
     * 
     * 
     * 
     */
    public class ProxyRule
    {

        public static ProxyRule Verificate(MethodInfo method, HttpProxyAttribute classAttr, IEnumerable<ProxyFilterAttribute> classFilters)
        {
            ProxyRule rule = new ProxyRule();
            HttpProxyContext.Builder contextBuilder = new HttpProxyContext.Builder();
            ResquestMappingAttribute methodAttr = method.GetCustomAttribute<ResquestMappingAttribute>();
            if (methodAttr == null)
            {
                methodAttr = new ResquestMappingAttribute();
            }

            ///TODO;拼接url

            SimpleUrl url;
            if (!string.IsNullOrEmpty(classAttr.Prefix))
            {
                url = new SimpleUrl(classAttr.Prefix);
                url.AppendPath(methodAttr.Path);
            }
            else
            {
                url = new SimpleUrl(methodAttr.Path);
            }

            if (string.IsNullOrEmpty(url.ToUrl()))
            {
                rule.IsValid = false;
                rule.ErrorMessage = $"方法{method.DeclaringType.FullName }.{method.Name}的Url无效";
                return rule;
            }

            //
            if (methodAttr.ContentType != "application/x-www-form-urlencoded" 
                && methodAttr.ContentType != "application/json")
            {

                rule.IsValid = false;
                rule.ErrorMessage = $"方法{method.DeclaringType.FullName }.{method.Name}的ContentType无效";
                return rule;
            }

            contextBuilder.SetAccept(methodAttr.Accept)
                .SetContentType(methodAttr.ContentType)
                .AddFilters(classFilters)
                .SetHttpMethod(methodAttr.Method)
                .SetMethodInfo(method)
                .SetTimeOut(methodAttr.TimeOut)
                .SetUrl(url);

            //Parameter
            List<ParameterInfo> queryArgs = new List<ParameterInfo>();
            List<ParameterInfo> pathArgs = new List<ParameterInfo>();
            List<ParameterInfo> bodyArgs = new List<ParameterInfo>();

            List<HttpArgument> arguments = new List<HttpArgument>();
            var ps = method.GetParameters();
            foreach (var p in ps)
            {
                HttpArgument argument;
                HttpArgumentAttribute argAttr = p.GetCustomAttribute<HttpArgumentAttribute>();


                if (p.IsOut)
                {
                    rule.IsValid = false;
                    rule.ErrorMessage = $"方法{method.DeclaringType.FullName }.{method.Name}参数{p.Name}有out";
                    return rule;
                }
                if (p.ParameterType.IsByRef)
                {
                    rule.IsValid = false;
                    rule.ErrorMessage = $"方法{method.DeclaringType.FullName }.{method.Name}参数{p.Name}有ref";
                    return rule;
                }
                if (p.GetCustomAttributes(typeof(ParamArrayAttribute), false).Count() > 0)
                {
                    rule.IsValid = false;
                    rule.ErrorMessage = $"方法{method.DeclaringType.FullName }.{method.Name}参数{p.Name}有params";
                    return rule;
                }


                if (argAttr == null)
                {
                    queryArgs.Add(p);
                    argument = new HttpArgument(p);
                    arguments.Add(argument);
                    continue;
                }

                argument = new HttpArgument(p,argAttr);
                arguments.Add(argument);

                switch (argAttr.ArgType)
                {
                    case HttpArgType.QueryString:
                        queryArgs.Add(p);
                        break;
                    case HttpArgType.Path:
                        pathArgs.Add(p);
                        break;
                    case HttpArgType.Body:
                        bodyArgs.Add(p);
                        break;
                    default:
                        queryArgs.Add(p);
                        break;
                }


            }


            if (methodAttr.Method == HttpMethod.POST)
            {
                if ("application/x-www-form-urlencoded" == methodAttr.ContentType)
                {
                    if (bodyArgs.Count > 1)
                    {
                        rule.IsValid = false;
                        rule.ErrorMessage = $"方法{method.DeclaringType.FullName }.{method.Name}无ContentType与参数不匹配";
                        return rule;
                    }
                }
            }
            contextBuilder.SetArguments(arguments);
            //Filter
            var filters = method.GetCustomAttributes<ProxyFilterAttribute>();
            contextBuilder.AddFilters(filters);

            rule.IsValid = true;
            rule.ProxyContext = contextBuilder.Build();
            return rule;

        }

        public bool IsValid { get; private set; }

        public HttpProxyContext ProxyContext { get; private set; }

        public string ErrorMessage { get; private set; }
    }
}
