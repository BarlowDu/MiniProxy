using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestMiniProxy
{
    [TestClass]
    public class UnitTestModifier
    {
        [TestMethod]
        public void Test() {

            ProxyBuilder.GetProxyObject(typeof(TestProxy), new DebugProxyHandler());
        }

        public class TestProxy
        {
            public virtual void TestRef(ref int a)
            {

            }

            public virtual void TestOut(out int a)
            {
                a = 1;
            }

            public virtual void TestParams(params int[] a) { }


            public virtual void TestComplex(int a, ref int b, out int c, params int[] arr)
            {
                c = 1;
            }
        }
    }
}
