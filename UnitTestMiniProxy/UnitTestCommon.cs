using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnitTestMiniProxy.Model;

namespace UnitTestMiniProxy
{
    [TestClass]
    public class UnitTestCommon
    {
        [TestMethod]
        public void TestNOTReturn()
        {
            var p = (TestProxy)ProxyBuilder.GetProxyObject(typeof(TestProxy), new DebugProxyHandler());
            p.TestNOTReturn(1, 2);

        }

        [TestMethod]
        public void TestReturn()
        {
            var p = (TestProxy)ProxyBuilder.GetProxyObject(typeof(TestProxy), new DebugProxyHandler());
            var r = p.TestReturn(1, 2);
            Assert.AreEqual(r, 3);

        }


        [TestMethod]
        public void TestReturnChange()
        {
            var p = (TestProxy)ProxyBuilder.GetProxyObject(typeof(TestProxy), new TestProxyHandler());
            var r = p.TestReturnChange();
            Assert.AreEqual(r.Id, 1001);
            Assert.AreEqual(r.Name, "a");

        }


        [TestMethod]
        public void TestRef()
        {
            var p = (TestProxy)ProxyBuilder.GetProxyObject(typeof(TestProxy), new DebugProxyHandler());
            SimpleModel a = new SimpleModel(1, "abc");
            p.TestRef(a);
            Assert.AreEqual(a.Id, 1001);

        }

        public class TestProxy
        {
            public virtual void TestNOTReturn(int a, int b) { }

            public virtual int TestReturn(int a, int b)
            {
                return a + b;
            }

            public virtual void TestRef(SimpleModel a)
            {
                a.Id = 1001;
            }
            public virtual SimpleModel TestReturnChange()
            {
                return new SimpleModel(1, "abc");
            }

        }

        public class TestProxyHandler : IProxyHandler
        {
            public void After(MethodInfo method, object retVal)
            {
                if (method.ReturnType == typeof(SimpleModel) || retVal != null)
                {
                    SimpleModel a = (SimpleModel)retVal;
                    a.Id = 1001;
                    a.Name = "a";
                }
            }

            public void Before(MethodInfo method, object[] args)
            {
            }
        }
    }


}
