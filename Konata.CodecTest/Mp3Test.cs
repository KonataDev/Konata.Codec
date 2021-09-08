using System.IO;
using NUnit.Framework;
using Konata.Codec.Audio;
using Konata.Codec.Audio.Codecs;

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
            // Create audio pipeline
            using var pipeline = new AudioPipeline
            {
                // Mp3 decoder stream
                new Mp3Codec.Decoder("audio/konata_test.mp3"),
                
                // Output file stream
                File.Open("audio/konata_test.mp3.pcm",
                    FileMode.Create, FileAccess.Write)
            };

            // Start pipeline
            if (!pipeline.Start().Result) Assert.Fail();
            {
                // Pass
                Assert.Pass();
            }
        }
    }
}
