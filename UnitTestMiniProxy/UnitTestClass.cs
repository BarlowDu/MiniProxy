using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniProxy;
using System.Collections.Generic;

namespace UnitTestMiniProxy
{
    [TestClass]
    public class UnitTestClass
    {
        [TestMethod]
        public void TestInfterface()
        {
            try
            {
                ProxyBuilder.GetProxyObject(typeof(IA), new DebugProxyHandler());
                Assert.Fail();
            }
            catch (Exception ex) {
                Assert.AreEqual(ex.Message, "ProxyBuilder不支持对抽象类与接口的代理");
            }
        }

        [TestMethod]
        public void TestAbstractClass()
        {
            try
            {
                ProxyBuilder.GetProxyObject(typeof(AbstractA), new DebugProxyHandler());
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "ProxyBuilder不支持对抽象类与接口的代理");
            }

        }


        [TestMethod]
        public void TestSealedClass()
        {
            try
            {
                ProxyBuilder.GetProxyObject(typeof(SealedA), new DebugProxyHandler());
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "ProxyBuilder不支持有sealed修饰的类");
            }

        }



        [TestMethod]
        public void TestGenericesClass()
        {
            try
            {
                ProxyBuilder.GetProxyObject(typeof(List<string>), new DebugProxyHandler());
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(ex.Message, "ProxyBuilder不支持泛型类");
            }

        }



        public interface IA { }

        public abstract class AbstractA { }

        public sealed class SealedA { }

        
    }
}
