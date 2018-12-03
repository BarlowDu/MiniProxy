using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniProxy
{
    public interface IMethodFilter
    {
        bool IsProxy(MethodInfo method);
    }
}
