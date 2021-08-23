using System.IO;
using NUnit.Framework;
using Konata.AvCodec;

namespace SilkCodec.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestEncode()
        {
            SilkDecoder.Decode("M:/Projects/KonataDev" +
                "/SilkCodec.Net/SilkCodec/in.silk", 24000, out var data);

            File.WriteAllBytes("outtt.pcm", data);

            Assert.Pass();
        }
    }
}