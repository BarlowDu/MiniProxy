using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniProxy
{
    public class DefaultMethodFilter:IMethodFilter
    {
        public bool IsProxy(MethodInfo method)
        {
            if (method.IsVirtual)
            {
                return true;
            }
            return false;
        }
    }
}
