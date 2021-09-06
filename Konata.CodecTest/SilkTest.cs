using System.IO;
using NUnit.Framework;
using Konata.Codec.Audio;

namespace Konata.CodecTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            if (!Directory.Exists("audio/"))
                Directory.CreateDirectory("audio/");

            File.Copy("../../../audio/konata_test.pcm", 
                "audio/konata_test.pcm", true);
        }

        [Test]
        public void TestSilkCodec()
        {
            var pcmData = File.ReadAllBytes("audio/konata_test.pcm");
            {
                Assert.True(SilkCodec.Encode(pcmData, 24000, out var silkData).Result);
                File.WriteAllBytesAsync("audio/konata_test.slk", silkData);

                Assert.True(SilkCodec.Decode(silkData, 24000, out var outPcmData).Result);
                File.WriteAllBytesAsync("audio/konata_test_codec.pcm", outPcmData);
            }
            Assert.Pass();
        }

        [Test]
        public void TestSilkCodeCodeCo()
        {
            var pcmData = File.ReadAllBytes("audio/konata_test.pcm");
            {
                for (var i = 0; i < 33; i++)
                {
                    Assert.True(SilkCodec.Encode(pcmData, 24000, out var silkData).Result);
                    Assert.True(SilkCodec.Decode(silkData, 24000, out pcmData).Result);
                }

                File.WriteAllBytesAsync("audio/konata_test_codecoddeco.pcm", pcmData);
            }
            Assert.Pass();
        }
    }
}
