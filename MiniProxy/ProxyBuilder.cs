using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace MiniProxy
{
    public class ProxyBuilder
    {
        static ConcurrentDictionary<TypeProxyKey, Type> PROXY_TYPES = new ConcurrentDictionary<TypeProxyKey, Type>(new TypeProxyKeyComparer());
        static int classid = 0;

        const string STATIC_METHODS_Name = "all_proxy_methods";
        const string HANDLER_NAME = "proxy_handler";
        const string PROXY_OBJECT_NAME = "proxy_object";
        const string PROXY_RESULT_NAME = "_proxy_result";


        private Type type;
        private IProxyHandler handler;
        private IMethodFilter methodFilter;

        private string className;
        private string classCode;
        private MethodInfo[] proxyMethods;

        public ProxyBuilder(Type type, IProxyHandler handler)
            : this(type, handler, new DefaultMethodFilter())
        {
        }


        public ProxyBuilder(Type type, IProxyHandler handler, IMethodFilter methodFilter)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (methodFilter == null)
            {
                throw new ArgumentNullException("methodFilter");
            }

            if (type.IsAbstract)
            {
                throw new TypeProxyException("ProxyBuilder不支持对抽象类与接口的代理");
            }

            if (type.IsSealed)
            {
                throw new TypeProxyException("ProxyBuilder不支持有sealed修饰的类");
            }


            if (type.IsGenericType)
            {
                throw new TypeProxyException("ProxyBuilder不支持泛型类");
            }

            this.type = type;
            this.handler = handler;
            this.methodFilter = methodFilter;
            TypeProxyKey key = new TypeProxyKey(type, handler, methodFilter);
            Type t;
            if (PROXY_TYPES.TryGetValue(key, out t))
            {
                className = t.FullName;
            }
            else
            {
                int id = Interlocked.Increment(ref classid);
                className = GetTypeName(type).Replace('.', '_') + "_Proxy" + "_" + id.ToString().Replace('-', '_');
            }
        }


        public static object GetProxyObject(Type type, IProxyHandler handler, IMethodFilter methodFilter)
        {
            return new ProxyBuilder(type, handler, methodFilter).GetProxy();
        }

        public static object GetProxyObject(Type type, IProxyHandler handler)
        {
            return new ProxyBuilder(type, handler).GetProxy();
        }

        public Type GetProxyType()
        {
            TypeProxyKey key = new TypeProxyKey(type, handler, methodFilter);
            Type result;
            if (PROXY_TYPES.TryGetValue(key, out result))
            {
                return result;
            }
            var provider = new CSharpCodeProvider();
            var options = new CompilerParameters();
            options.GenerateInMemory = true;


            Assembly[] AssbyCustmList = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in AssbyCustmList)
            {
                if (a.IsDynamic == false)
                {
                    options.ReferencedAssemblies.Add(a.Location);
                }
            }


            var results = provider.CompileAssemblyFromSource(
                       options, GetCode());
            if (!results.Errors.HasErrors)
            {
                var proxyType = results.CompiledAssembly.GetType(className);
                var field = proxyType.GetField(STATIC_METHODS_Name, BindingFlags.Static | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(null, proxyMethods);
                }
                result = PROXY_TYPES.GetOrAdd(key, proxyType);
                return result;
            }
            else
            {
                StringBuilder err = new StringBuilder();
                for (int i = 0; i < results.Errors.Count; i++)
                {
                    var e = results.Errors[i];
                    err.AppendLine(e.ErrorText);
                }
                throw new TypeProxyException(err.ToString());
            }
        }


        public object GetProxy()
        {
            var proxyType = GetProxyType();
            var constructor = proxyType.GetConstructor(new Type[] { typeof(IProxyHandler) });
            return constructor.Invoke(new object[] { handler });
        }



        public string GetCode()
        {
            if (classCode != null)
            {
                return classCode;
            }

            MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.InvokeMethod);
            StringBuilder code = new StringBuilder();
            //code.AppendLine("using TestConsoleApplication;");
            code.Append("public class ").Append(className).Append(':').Append(GetTypeName(type)).Append("{").AppendLine();
            code.Append("private ").Append(GetTypeName(type)).Append(" ").Append(PROXY_OBJECT_NAME)
                .Append("=new ").Append(GetTypeName(type)).Append("();").AppendLine();

            code.Append("private ").Append(typeof(IProxyHandler).FullName).Append(" ").Append(HANDLER_NAME).Append(";").AppendLine();


            code.Append("public ").Append(className).Append('(').Append(typeof(IProxyHandler).FullName).Append(" handler){")
                .AppendLine("this.").Append(HANDLER_NAME).Append("=handler;").AppendLine("}");


            code.Append("private static ").Append(GetTypeName(typeof(MethodInfo))).Append("[] ").Append(STATIC_METHODS_Name).Append(";");

            List<MethodInfo> filterMethods = new List<MethodInfo>();

            foreach (var method in methods)
            {
                if (methodFilter.IsProxy(method))
                {
                    filterMethods.Add(method);
                }

            }
            proxyMethods = filterMethods.ToArray();
            for (int i = 0; i < proxyMethods.Length; i++)
            {
                code.Append(GetMethodCode(proxyMethods[i], i));
            }


            code.Append("}");

            classCode = code.ToString();
            return classCode;
        }

        private string GetMethodCode(MethodInfo method, int index)
        {
            StringBuilder code = new StringBuilder();
            #region init
            bool hasReturn = true;
            List<StructParam> parameters = new List<StructParam>();

            if (method.ReturnType == typeof(void))
            {
                hasReturn = false;
            }



            foreach (var p in method.GetParameters())
            {
                parameters.Add(new StructParam(p));
            }
            #endregion

            #region method declare
            code.Append("public override ");
            if (hasReturn)
            {
                code.Append(GetTypeName(method.ReturnType));
            }
            else
            {
                code.Append("void");
            }
            code.Append(' ').Append(method.Name);
            if (method.IsGenericMethod)
            {
                code.Append('<');
                foreach (var g in method.GetGenericArguments())
                {
                    code.Append(GetTypeName(g)).Append(',');
                }
                code.Remove(code.Length - 1, 1).Append('>');
            }
            code.Append('(');
            foreach (var p in parameters)
            {
                switch (p.Status)
                {
                    case ParamStatus.Out:
                        code.Append("out ");
                        break;
                    case ParamStatus.Ref:
                        code.Append("ref ");
                        break;
                    case ParamStatus.Params:
                        code.Append("params ");
                        break;
                    default:
                        break;
                }
                code.Append(p.TypeName).Append(' ').Append(p.Name).Append(',');
            }
            if (parameters.Count > 0)
            {
                code.Remove(code.Length - 1, 1);
            }
            code.Append(')').AppendLine().Append("{").AppendLine();
            #endregion

            #region method body
            if (hasReturn)
            {
                code.Append(GetTypeName(method.ReturnType)).Append(" ").Append(PROXY_RESULT_NAME).Append(";").AppendLine();
            }


            //code.Append("var method=").Append(STATIC_METHODS_Name).Append("[").Append(index).Append("];").AppendLine();

            foreach (var p in parameters)
            {
                if (p.Status == ParamStatus.Out)
                {
                    code.Append(p.Name).Append('=').Append(GetDefaultValue(p.TypeInfo)).Append(';').AppendLine();
                };
            }
            /*
            code.Append("object[] arguments=new object[").Append(parameters.Count).Append("];").AppendLine();
            for (int i = 0; i < parameters.Count; i++)
            {
                code.Append("arguments[").Append(i).Append("]=").Append(parameters[i].Name).Append(';').AppendLine();
            }
            */


            code.Append(HANDLER_NAME).Append(".Before(").Append(STATIC_METHODS_Name).Append("[").Append(index).Append("],");

            code.Append("new object[]{");
            foreach (var p in parameters)
            {
                code.Append(p.Name).Append(',');
            }
            if (parameters.Count > 0)
            {
                code.Remove(code.Length - 1, 1).Append("});").AppendLine();
            }
            else
            {
                code.Append("});").AppendLine();
            }

            if (hasReturn)
            {
                code.Append(PROXY_RESULT_NAME).Append("=");
            }
            code.Append(PROXY_OBJECT_NAME).Append(".").Append(method.Name).Append('(');

            foreach (var p in parameters)
            {

                switch (p.Status)
                {
                    case ParamStatus.Out:
                        code.Append("out ");
                        break;
                    case ParamStatus.Ref:
                        code.Append("ref ");
                        break;
                    default:
                        break;
                }
                code.Append(p.Name).Append(',');
            }
            if (parameters.Count > 0)
            {
                code.Remove(code.Length - 1, 1);
            }
            code.Append(");").AppendLine();
            if (hasReturn)
            {
                code.Append(HANDLER_NAME).Append(".After(").Append(STATIC_METHODS_Name).Append("[").Append(index).Append("]").Append(",").Append(PROXY_RESULT_NAME).Append(");").AppendLine();
                code.AppendLine("return ").Append(PROXY_RESULT_NAME).Append(";");
            }
            else
            {
                code.Append(HANDLER_NAME).Append(".After(").Append(STATIC_METHODS_Name).Append("[").Append(index).Append("]").Append(",null);").AppendLine();

            }
            code.AppendLine("}");

            #endregion
            return code.ToString();
        }


        private string GetDefaultValue(Type t)
        {
            if (t == typeof(bool))
            {
                return "false";
            }
            if (t == typeof(char))
            {
                return "\0";
            }
            if (t == typeof(sbyte) || t == typeof(short) || t == typeof(int) ||
                t == typeof(long) || t == typeof(byte) || t == typeof(ushort) ||
                t == typeof(uint) || t == typeof(ulong) || t == typeof(float) ||
                 t == typeof(double) || t == typeof(decimal))
            {
                return "0";
            }



            return "null";


        }

        private static string GetTypeName(Type t)
        {
            if (t.IsArray)
            {
                return GetTypeName(t.GetElementType()) + "[]";
            }
            if (t.IsNested) {
                return GetTypeName(t.DeclaringType) + "." + t.Name;
            }
            if (t.IsGenericType == false)
            {
                return t.FullName ?? t.Name;//
            }


            StringBuilder result = new StringBuilder();
            int i = t.Name.LastIndexOf('`');
            result.Append(t.Namespace).Append('.').Append(t.Name.Substring(0, i));
            result.Append('<');

            foreach (var p in t.GetGenericArguments())
            {
                result.Append(GetTypeName(p)).Append(',');
            }
            result.Remove(result.Length - 1, 1);
            result.Append('>');
            return result.ToString();
        }
        class StructParam
        {

            public StructParam(ParameterInfo parameter)
            {
                this.TypeInfo = parameter.ParameterType;
                this.Name = parameter.Name;
                this.ParameterInfo = parameter;


                Status = ParamStatus.Default;

                //TypeName = GetTypeName(TypeInfo);
                //参数有ref,out修饰类型名都以&结束
                if (parameter.IsOut)
                {
                    Status = ParamStatus.Out;
                    this.TypeInfo = this.TypeInfo.GetElementType();
                }
                else
                {
                    if (this.TypeInfo.IsByRef)
                    {
                        Status = ParamStatus.Ref;
                        this.TypeInfo = this.TypeInfo.GetElementType();
                    }
                }
                if (parameter.GetCustomAttributes(typeof(ParamArrayAttribute), false).Count() > 0)
                {
                    this.Status = ParamStatus.Params;
                }

                this.TypeName = GetTypeName(TypeInfo);

            }

            public Type TypeInfo { get; }
            public string Name { get; }
            public ParamStatus Status { get; }
            public ParameterInfo ParameterInfo { get; }

            public string TypeName { get; }
            //public string TypeNameRef { get; set; }



        }

        /// <summary>
        /// 参数修饰符out,ref,params
        /// </summary>
        enum ParamStatus
        {

            Default,
            Out,
            Ref,
            Params
        }

        class TypeProxyKey
        {
            public TypeProxyKey(Type type, IProxyHandler handler, IMethodFilter filter)
            {
                Type = type;
                Handler = handler;
                Filter = filter;

                //ConcurrentDictionary<TypeProxy,Type> di=new ConcurrentDictionary<TypeProxy, Type>(IEqualityComparer)
            }

            public Type Type { get; }
            public IProxyHandler Handler { get; }
            public IMethodFilter Filter { get; }

            public Type ProxyType { get; set; }
        }

        class TypeProxyKeyComparer : IEqualityComparer<TypeProxyKey>
        {
            public bool Equals(TypeProxyKey x, TypeProxyKey y)
            {
                if (x == null || y == null)
                {
                    return false;
                }
                if (x.Type == y.Type && x.Handler == y.Handler && x.Filter == y.Filter)
                {
                    return true;
                }
                return false;
            }

            public int GetHashCode(TypeProxyKey obj)
            {
                //throw new NotImplementedException();
                return obj.Type.GetHashCode() ^ obj.Handler.GetHashCode() ^ obj.Filter.GetHashCode();
            }
        }

    }
}
