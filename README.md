## Konata.Codec

[![Codec](https://img.shields.io/badge/Konata-Codec-blue)](#)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-orange)](https://www.nuget.org/packages/Konata.Codec/)
[![License](https://img.shields.io/static/v1?label=LICENSE&message=GNU%20GPLv3&color=lightrey)](./blob/main/LICENSE)

Audio & Video codec library for Konata.

### Codecs

| Codec | Link |
| ----- | ---- |
| Wave Codec | Internal |
| Mp3 Codec | [MP3Sharp](https://github.com/ZaneDubya/MP3Sharp) |
| Silk Codec | [libSilkCodec](https://github.com/KonataDev/libSilkCodec) |

### Audio Resampler

Konata.Codec has an internal simple audio resampler.  
It can help you to sample the pcm data to any supported format.

It's very easy to use.

```C#
// Create audio pipeline
using var pipeline = new AudioPipeline
{
    // Mp3 stream
    mp3DecodeStream,

    // Resample mp3 to default
    new AudioResampler(AudioInfo.Default()),
    
    // Adaptive audio pipeline
    // You don't need to care about the upstream format
    // Just tell resampler what should it do in next
    new AudioResampler(new AudioInfo
        (AudioFormat.Signed16Bit, AudioChannel.Mono, 23333)),

    // Output pcm stream
    outputStream
};

// Start pipeline
pipeline.Start();
```

## Todo
- [x] Streaming methods
- [ ] AudioResampler can cause audio distortion

## LICENSE

Licensed under GNU GPLv3 with ❤.
