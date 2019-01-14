using HttpClientProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTestHttpClientProxy
{
    [TestClass]
    public class TestSimpleUrl
    {
        [TestMethod]
        public void TestProtocol()
        {

            SimpleUrl url;
            url = new SimpleUrl("http://dutc.com");
            Assert.AreEqual("http", url.Protocol);


            url = new SimpleUrl("https://dutc.com");
            Assert.AreEqual("https", url.Protocol);
        }

        [TestMethod]
        public void TestHost()
        {

            SimpleUrl url;
            url = new SimpleUrl("http://dutc.com");
            Assert.AreEqual("dutc.com", url.Host);


            url = new SimpleUrl("https://localhost");
            Assert.AreEqual("localhost", url.Host);

            url = new SimpleUrl("https://127.0.0.1");
            Assert.AreEqual("127.0.0.1", url.Host);


            url = new SimpleUrl("https://{host}");
            Assert.AreEqual("{host}", url.Host);

            ////////////////////////////////
            url = new SimpleUrl("http://dutc.com/");
            Assert.AreEqual("dutc.com", url.Host);

            url = new SimpleUrl("https://localhost/");
            Assert.AreEqual("localhost", url.Host);

            url = new SimpleUrl("https://127.0.0.1/");
            Assert.AreEqual("127.0.0.1", url.Host);


            url = new SimpleUrl("https://{host}/");
            Assert.AreEqual("{host}", url.Host);

            ////////////////////////////////
            url = new SimpleUrl("http://dutc.com:8000/");
            Assert.AreEqual("dutc.com", url.Host);

            url = new SimpleUrl("https://localhost:8000/");
            Assert.AreEqual("localhost", url.Host);

            url = new SimpleUrl("https://127.0.0.1:8000/");
            Assert.AreEqual("127.0.0.1", url.Host);


            url = new SimpleUrl("https://{host}:8000/");
            Assert.AreEqual("{host}", url.Host);



        }

        [TestMethod]
        public void TestPort()
        {

            SimpleUrl url;
            url = new SimpleUrl("http://dutc.com");
            Assert.AreEqual(0, url.Port);


            url = new SimpleUrl("http://dutc.com/");
            Assert.AreEqual(0, url.Port);


            url = new SimpleUrl("http://dutc.com:80");
            Assert.AreEqual(80, url.Port);

            url = new SimpleUrl("http://dutc.com:80/");
            Assert.AreEqual(80, url.Port);
        }

        [TestMethod]
        public void TestPathLength()
        {
            SimpleUrl url;
            url = new SimpleUrl("http://dutc.com/a/b/c");
            Assert.AreEqual(3, url.Paths.Count);
            Assert.AreEqual("/a/b/c", url.Path);


            url = new SimpleUrl("http://dutc.com/a/b/c/");
            Assert.AreEqual(4, url.Paths.Count);
            Assert.AreEqual("/a/b/c/", url.Path);


            url = new SimpleUrl("http://dutc.com/a/b/c?a=a");
            Assert.AreEqual(3, url.Paths.Count);
            Assert.AreEqual("/a/b/c", url.Path);


            url = new SimpleUrl("http://dutc.com/a/b/c/?a=a");
            Assert.AreEqual(4, url.Paths.Count);
            Assert.AreEqual("/a/b/c/", url.Path);

        }

        [TestMethod]
        public void TestQueryString()
        {

        }

        [TestMethod]
        public void TestAppendPath() {

            SimpleUrl url;
            url = new SimpleUrl("http://dutc.com/a/b/c");
            url.AppendPath("d/e/f");
            Assert.AreEqual(6, url.Paths.Count);
            Assert.AreEqual("/a/b/c/d/e/f", url.Path);


            url = new SimpleUrl("http://dutc.com/a/b/c");
            url.AppendPath("/d/e/f");
            Assert.AreEqual(6, url.Paths.Count);
            Assert.AreEqual("/a/b/c/d/e/f", url.Path);
            

            url = new SimpleUrl("http://dutc.com/a/b/c/");
            url.AppendPath("d/e/f");
            Assert.AreEqual(6, url.Paths.Count);
            Assert.AreEqual("/a/b/c/d/e/f", url.Path);


            url = new SimpleUrl("http://dutc.com/a/b/c/");
            url.AppendPath("/d/e/f");
            Assert.AreEqual(6, url.Paths.Count);
            Assert.AreEqual("/a/b/c/d/e/f", url.Path);


            url = new SimpleUrl("http://dutc.com/a/b/c?a=a");
            url.AppendPath("/d/e/f");
            Assert.AreEqual(6, url.Paths.Count);
            Assert.AreEqual("/a/b/c/d/e/f", url.Path);


            url = new SimpleUrl("http://dutc.com/a/b/c/?a=a");
            url.AppendPath("/d/e/f");
            Assert.AreEqual(6, url.Paths.Count);
            Assert.AreEqual("/a/b/c/d/e/f", url.Path);
        }

        [TestMethod]
        public void TestPathParameter() {

            SimpleUrl url;
            url = new SimpleUrl("http://dutc.com/a/{phone}/c");
            url.SetPathParameter("phone", "13010005000");
            Assert.AreEqual("/a/13010005000/c", url.Path);


            url = new SimpleUrl("http://dutc.com/a/{phone}/c/{phone}/");
            url.SetPathParameter("phone", "13010005000");
            Assert.AreEqual("/a/13010005000/c/13010005000/", url.Path);
        }

    }
}
