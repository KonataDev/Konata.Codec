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
    public void TestWavEncode()
    {
        // Create audio pipeline
        using var pipeline = new AudioPipeline
        {
            File.Open("audio/konata_test.pcm", FileMode.Open, FileAccess.Read),
            new WavCodec.Encoder(AudioInfo.SilkV3()),
            File.Open("audio/konata_test.wav", FileMode.Create, FileAccess.Write)
        };

        // Start pipeline
        if (!pipeline.Start().Result) Assert.Fail();
        {
            // Pass
            Assert.Pass();
        }
    }

    [Test]
    public void TestWavDecode()
    {
        using var pipeline = new AudioPipeline
        {
            new WavCodec.Decoder("audio/konata_test.wav"),
            new AudioResampler(new AudioInfo(AudioFormat.Signed16Bit, AudioChannel.Mono, 44100)),
            File.Open("audio/konata_test.pcm", FileMode.Create, FileAccess.Write)
        };

        if (!pipeline.Start().Result) Assert.Fail();
        Assert.Pass();
    }
}
