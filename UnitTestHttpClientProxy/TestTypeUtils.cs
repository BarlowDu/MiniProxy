using HttpClientProxy.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestHttpClientProxy
{
    [TestClass]
    public class TestTypeUtils
    {

        [TestMethod]
        public void TestBaseType() {

            Assert.IsFalse(TypeUtils.IsModel(typeof(bool)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(char)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(byte)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(short)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(int)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(long)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(ushort)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(uint)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(ulong)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(float)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(double)));
            Assert.IsFalse(TypeUtils.IsModel(typeof(decimal)));


        }

        [TestMethod]
        public void TestString()
        {
            Assert.IsFalse(TypeUtils.IsModel(typeof(string)));
        }

        [TestMethod]
        public void TestArray()
        {
            Assert.IsTrue(TypeUtils.IsModel(typeof(int[])));
            Assert.IsTrue(TypeUtils.IsModel(typeof(decimal[])));
            Assert.IsTrue(TypeUtils.IsModel(typeof(string[])));
        }

        [TestMethod]
        public void TestObject() {
            Assert.IsTrue(TypeUtils.IsModel(typeof(TypeUtils)));
        }
    }
}
