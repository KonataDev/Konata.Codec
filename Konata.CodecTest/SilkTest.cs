using System.IO;
using NUnit.Framework;
using Konata.Codec.Audio;
using Konata.Codec.Audio.Codecs;

namespace Konata.CodecTest;

public class SilkTest
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

            new AudioResampler(new AudioInfo(AudioFormat.Signed16Bit, AudioChannel.Mono, 44100),
                new AudioInfo(AudioFormat.Signed16Bit, AudioChannel.Mono, 24000)),

            // Mp3 decoder stream
            new SilkV3Codec.Encoder(),

            // Output file stream
            File.Open("audio/konata_test.slk",
                FileMode.Create, FileAccess.Write)
        };

        // Start pipeline
        if (!pipeline.Start().Result) Assert.Fail();
        {
            // Pass
            Assert.Pass();
        }
    }

    [Test]
    public void TestSilkDecode()
    {
        // Create audio pipeline
        using var pipeline = new AudioPipeline
        {
            // Input file stream
            File.Open("audio/konata_test.slk",
                FileMode.Open, FileAccess.Read),

            // Mp3 decoder stream
            new SilkV3Codec.Decoder(),

            new WavCodec.Encoder(),
            // Output file stream
            File.Open("audio/konata_test.slk.pcm",
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
