using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NicoNicoDownloader;

namespace Test
{
    [TestClass]
    public class ConverterTest
    {
        [TestMethod]
        public void Test1()
        {
            var converter = new TitileConverterInfo();
            converter.InputPattern = new List<string>()
            {
                "【.+】(?<title>.+)"
            }.ToArray();
            converter.OutputParttern = "_%title%";
            Assert.AreEqual( "_a", converter.ConvertTitle("【test】a"));
        }
    }
}
