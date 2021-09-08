using System.IO;
using NUnit.Framework;
using Konata.Codec.Audio;

namespace Konata.CodecTest
{
    public class PipelineTest
    {
        [Test]
        public void TestPipelineCopy()
        {
            using var pipeline = new AudioPipeline
            {
                // Input file stream
                File.Open("audio/konata_test.mp3",
                    FileMode.Open, FileAccess.Read),

                // Output file stream
                File.Open("audio/konata_test.mp3.pcm",
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
