using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnitTestMiniProxy.Model;

namespace UnitTestMiniProxy
{
    [TestClass]
    public class UnitTestGenerics
    {
        [TestMethod]
        public void Test() {
            ProxyBuilder.GetProxyObject(typeof(TestProxy), new DebugProxyHandler());
        }


        public class TestProxy
        {
            public virtual void TestSimpleGenerices(List<int> list) { }

            public virtual List<SimpleModel> TestSimpleGenericesReturn()
            {
                return new List<SimpleModel>();
            }

            public virtual void TestComplexGenerices(Dictionary<string, List<int>> dic) { }

            public virtual Dictionary<string, List<int>> TestComplexGenericesReturn()
            {
                return new Dictionary<string, List<int>>();
            }

            public virtual int TestLambda(int[] arr, Func<int[], int> func)
            {
                return func(arr);
            }
        }

    }
}
