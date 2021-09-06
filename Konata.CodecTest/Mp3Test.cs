using System.IO;
using NUnit.Framework;
using Konata.Codec.Audio;

namespace Konata.CodecTest
{
    public class Mp3Test
    {
        [SetUp]
        public void Setup()
        {
            if (!Directory.Exists("audio/"))
                Directory.CreateDirectory("audio/");

            File.Copy("../../../audio/konata_test.mp3",
                "audio/konata_test.mp3", true);
        }

        [Test]
        public void TestMp3Decode()
        {
            var mp3Data = File.ReadAllBytes("audio/konata_test.mp3");
            {
                Assert.True(Mp3Codec.Decode(mp3Data, out var pcmData).Result);
                File.WriteAllBytesAsync("audio/konata_test.mp3.pcm", pcmData);
            }
            Assert.Pass();
        }
    }
}
