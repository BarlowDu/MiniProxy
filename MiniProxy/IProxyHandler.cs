using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniProxy
{
    public interface IProxyHandler
    {
        void Before(MethodInfo method, object[] args);
        void After(MethodInfo method, object retVal);
    }
}
