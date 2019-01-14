using HttpClientProxy.Attr;
using HttpClientProxy.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy.Context
{
    public class HttpArgument
    {
        public HttpArgType ArgType { get; set; }
        public ParameterInfo ParameterInfo { get; set; }

        public string ParameterName { get; set; }
        public string HttpArgName { get; set; }

        public object Value { get; set; }

        private HttpArgument() { }

        public HttpArgument(ParameterInfo p)
        {
            ArgType = HttpArgType.QueryString;
            ParameterInfo = p;
            ParameterName = p.Name;
            HttpArgName = ParameterName;
        }

        public HttpArgument(ParameterInfo p, HttpArgumentAttribute argAttr)
        {


            ArgType = argAttr.ArgType;
            ParameterInfo = p;
            ParameterName = p.Name;
            HttpArgName = string.IsNullOrEmpty(argAttr.HttpArgName) ? ParameterName : argAttr.HttpArgName;
        }

        public HttpArgument(HttpArgument arg) : this(arg, null)
        {

        }

        public HttpArgument(HttpArgument arg, object value)
        {
            ArgType = arg.ArgType;
            ParameterInfo = arg.ParameterInfo;
            ParameterName = arg.ParameterName;
            HttpArgName = arg.HttpArgName;
            Value = value;
        }
    }
}
