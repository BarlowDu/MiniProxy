using HttpClientProxy.Attr;
using HttpClientProxy.Context;
using HttpClientProxy.Filter;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace HttpClientProxy
{
    public class HttpProxyBuilder
    {
        static int classid = 0;

        static ConcurrentDictionary<Type, Type> PROXY_TYPES = new ConcurrentDictionary<Type, Type>();

        const string CONTEXTS_NAME = "Contexts";
        const string Dispatcher_Name = "Dispatcher";

        /******************************/

        private List<HttpProxyContext> contexts = new List<HttpProxyContext>();
        private string className;

        Type proxyType;

        Type clientInterface;

        private HttpProxyBuilder(Type _clientInterface)
        {
            this.clientInterface = _clientInterface;
        }




        public static Type GetProxyType<T>()
        {
            Type type = typeof(T);
            HttpProxyBuilder builder = new HttpProxyBuilder(type);
            builder.Build();
            PROXY_TYPES.GetOrAdd(type, builder.proxyType);
            return builder.proxyType;

        }


        public static T GetProxy<T>()
        {
            Type type = GetProxyType<T>();

            var construct = type.GetConstructor(new Type[] { });
            return (T)construct.Invoke(new object[] { });
        }



        public void Build()
        {
            if (clientInterface == null)
            {
                throw new ArgumentNullException();
            }
            if (clientInterface.IsInterface == false)
            {
                throw new Exception();
            }
            RuleCheck();

            int id = Interlocked.Increment(ref classid);
            className = GetTypeName(clientInterface).Replace('.', '_') + "_Proxy" + "_" + id.ToString().Replace('-', '_');
            string code = getCode(clientInterface);

            //编译
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
                       options, code);
            if (!results.Errors.HasErrors)
            {
                proxyType = results.CompiledAssembly.GetType(className);
                var field = proxyType.GetField(CONTEXTS_NAME, BindingFlags.Static | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(null, contexts);
                }


                field = proxyType.GetField(Dispatcher_Name, BindingFlags.Static | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(null, new HttpClientDispatcher());
                }

            }
            else
            {
                StringBuilder err = new StringBuilder();
                for (int i = 0; i < results.Errors.Count; i++)
                {
                    var e = results.Errors[i];
                    err.AppendLine(e.ErrorText);
                }
                throw new Exception(err.ToString());
            }
        }



        public void RuleCheck()
        {

            HttpProxyAttribute attr = clientInterface.GetCustomAttribute<HttpProxyAttribute>();
            if (attr == null)
            {
                throw new Exception();
            }

            var filters = clientInterface.GetCustomAttributes<ProxyFilterAttribute>();

            var methods = clientInterface.GetMethods();
            foreach (var m in methods)
            {
                var rule = ProxyRule.Verificate(m, attr, filters);
                if (rule.IsValid == false)
                {
                    throw new Exception(rule.ErrorMessage);
                }
                contexts.Add(rule.ProxyContext);
            }


        }

        #region Code
        private string getCode(Type clientInterface)
        {
            StringBuilder code = new StringBuilder();
            code.Append("public class ").Append(className).Append(":").Append(GetTypeName(clientInterface));
            code.AppendLine("{");

            code.Append("private static ").Append(GetTypeName(typeof(List<HttpProxyContext>))).Append(' ').Append(CONTEXTS_NAME).Append(";").AppendLine();

            code.Append("private static ").Append(GetTypeName(typeof(HttpClientDispatcher))).Append(' ').Append(Dispatcher_Name).Append(";").AppendLine();
            var methods = clientInterface.GetMethods();
            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                code.AppendLine(GetMethodCode(method, i));
            }
            code.AppendLine("}");
            return code.ToString();
        }

        private string GetMethodCode(MethodInfo method, int index)
        {
            StringBuilder code = new StringBuilder();

            #region init
            bool hasReturn = true;
            if (method.ReturnType == typeof(void))
            {
                hasReturn = false;
            }

            code.Append("public ");
            if (hasReturn)
            {
                code.Append(GetTypeName(method.ReturnType)).Append(" ");
            }
            else
            {
                code.Append("void ");
            }

            code.Append(method.Name).Append("(");
            var ps = method.GetParameters();

            foreach (var param in ps)
            {

                code.Append(GetTypeName(param.ParameterType)).Append(' ').Append(param.Name).Append(',');
            }
            if (ps.Length > 0)
            {
                code.Remove(code.Length - 1, 1);
            }
            code.Append("){").AppendLine();
            #endregion

            #region body

            if (hasReturn)
            {
                code.AppendLine("object _result=");
            }
            code.Append(className).Append('.').Append(Dispatcher_Name).Append(".Request(")
                .Append(className).Append('.').Append(CONTEXTS_NAME).Append('[').Append(index).Append("],")
                .Append("new object[]{");
            foreach (var p in ps)
            {
                code.Append(p.Name).Append(',');
            }
            if (ps.Length > 0)
            {
                code.Remove(code.Length - 1, 1);
            }
            code.Append("},this);").AppendLine();

            if (hasReturn)
            {
                code.AppendLine("if(_result==null){return null;}return (")
                    .Append(GetTypeName(method.ReturnType)).Append(")_result;");
            }
            #endregion

            code.AppendLine("}");
            return code.ToString();
        }

        private static string GetTypeName(Type t)
        {
            if (t.IsArray)
            {
                return GetTypeName(t.GetElementType()) + "[]";
            }
            if (t.IsNested)
            {
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
    }
    #endregion

}
