using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniProxy
{
    public class DebugProxyHandler:IProxyHandler
    {
        public void Before(MethodInfo method, object[] args)
        {
            Debug.WriteLine("Proxy before");
            if (args.Length > 0)
            {
                foreach (var obj in args)
                {
                    Debug.WriteLine(obj);
                }
            }
            Debug.WriteLine(new string('-', 20));
        }
        public void After(MethodInfo method, object retVal)
        {
            Console.WriteLine(method.Name);
            Debug.WriteLine(new string('*', 20));
            if (retVal != null)
            {
                //if (retVal.GetType() == typeof(int)) {
                //    retVal = (int)retVal * 100;
                //}
                Debug.WriteLine(retVal.GetType());
            }
            Debug.WriteLine("Proxy after");
        }
    }
}
