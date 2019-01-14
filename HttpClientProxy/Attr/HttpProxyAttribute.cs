using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy.Attr
{
    public class HttpProxyAttribute:Attribute
    {
        public string Prefix { get; set; }

    }
}
