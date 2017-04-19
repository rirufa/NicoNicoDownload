using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NicoNicoDownloader.Model;

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
                ".+／(?<title>.+)",
                "【(?<album_artist>.+)】(?<title>.+)"
            }.ToArray();
            converter.OutputParttern = "%album_artist%_%title%";
            Assert.AreEqual( "test_a", converter.ConvertTitle("【test】a"));
            Assert.AreEqual("_a", converter.ConvertTitle("test／a"));
        }

        [TestMethod]
        public void Test2()
        {
            var converter = new TitileConverterInfo();
            converter.InputPattern = new List<string>()
            {
                ".+／(?<title>.+)",
                "【(?<album_artist>.+)】(?<title>.+)"
            }.ToArray();
            converter.KnownBandList = new List<string>()
            {
                "サークル:(.+)",
                "test"
            }.ToArray();
            converter.OutputParttern = "%known_album_artist%___%title%";
            Assert.AreEqual("test___a", converter.ConvertTitle("【test】a"));
            Assert.AreEqual("test___a", converter.ConvertTitle("test／a"));
            Assert.AreEqual("test___a", converter.ConvertTitle("vocal／a", "サークル:test"));
            Assert.AreEqual("test___a", converter.ConvertTitle("vocal／a", "testのa"));
        }

    }
}
