using System.IO;
using NUnit.Framework;
using Konata.Codec.Audio;
using Konata.Codec.Audio.Codecs;

namespace Konata.CodecTest
{
    public class ResampleTest
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
        public void TestResample()
        {
            // Create audio pipeline
            using var pipeline = new AudioPipeline
            {
                // Input file stream
                File.Open("audio/konata_test.pcm",
                    FileMode.Open, FileAccess.Read),

                // Audio resampler
                new AudioResampler(AudioInfo.SilkV3(), AudioInfo.Default()),

                // Output file stream
                File.Open("audio/konata_test.resample.pcm",
                    FileMode.OpenOrCreate, FileAccess.Write)
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
