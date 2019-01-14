using HttpClientProxy.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy.Attr
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class HttpArgumentAttribute : Attribute
    {
        public string HttpArgName { get; set; }
        public HttpArgType ArgType { get; set; }

        //public string Defalut { get; set; }
    }
}
