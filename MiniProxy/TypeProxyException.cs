using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MiniProxy
{
    public class TypeProxyException : Exception
    {
        public TypeProxyException(string msg) : base(msg) { }

        public static TypeProxyException GetException(string msg, Type type)
        {
            return GetException(msg, type, null);
        }

        public static TypeProxyException GetException(string msg, Type type, MethodInfo method)
        {
            return GetException(msg, type, method, null);

        }
        public static TypeProxyException GetException(string msg, Type type, MethodInfo method, ParameterInfo parameter)
        {
            StringBuilder message = new StringBuilder(msg);
            message.Append('(');
            if (type != null)
            {
                message.Append("type:").Append(type.FullName).Append(',');
            }
            if (method != null)
            {
                message.Append("method:").Append(method.Name).Append(',');
            }
            if (parameter != null)
            {
                message.Append("parameter:").Append(parameter.Name).Append(',');
            }

            message.Append(')');
            return new TypeProxyException(message.ToString());
        }
    }
}
