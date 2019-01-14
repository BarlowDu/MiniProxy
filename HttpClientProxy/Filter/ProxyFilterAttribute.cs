using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientProxy.Filter
{
    public abstract class ProxyFilterAttribute:Attribute
    {
        public abstract void Before();
        public abstract void After();
    }
}
