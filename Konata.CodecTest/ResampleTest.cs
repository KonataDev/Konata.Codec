using System.IO;
using NUnit.Framework;
using Konata.Codec;

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
            var pcmData = File.ReadAllBytes("audio/konata_test.pcm");
            {
                var sampler = new AudioResampler(pcmData);
                {
                    // Set configs
                    sampler.SetAlgorithm(Algorithm.Direct);
                    sampler.SetOrigin(24000, 1, Format.Signed16Bit);
                    sampler.SetTarget(44100, 2, Format.UnSigned8Bit);

                    // Resample
                    if (sampler.Resample(out var data).Result)
                    {
                        File.WriteAllBytes("audio/konata_test.pcm.resample", data);
                        Assert.Pass();
                    }

                    // Failed
                    Assert.Fail();
                }
            }
        }
    }
}
