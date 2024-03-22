using System.IO;
using NUnit.Framework;
using Konata.Codec.Audio;
using Konata.Codec.Audio.Codecs;

namespace Konata.CodecTest;

public class WavTest
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
    public void TestSilkEncode()
    {
        // Create audio pipeline
        using var pipeline = new AudioPipeline
        {
            // Input file stream
            File.Open("audio/konata_test.pcm",
                FileMode.Open, FileAccess.Read),

            // Mp3 decoder stream
            new WavCodec.Encoder(AudioInfo.SilkV3()),

            // Output file stream
            File.Open("audio/konata_test.wav",
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
