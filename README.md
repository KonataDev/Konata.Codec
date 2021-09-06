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
var sampler = new AudioResampler(pcmData);
{
    // Set input format
    // 24000 Hz, 1 Channel(Mono), Signed 16 bit
    sampler.SetOrigin(24000, 1, Format.Signed16Bit);
    
    // Set output format
    // 44100 Hz, 2 Channel(Stereo), Signed 16 bit
    sampler.SetTarget(44100, 2, Format.Signed16Bit);

    // Start resample
    if (sampler.Resample(out var data).Result)
        File.WriteAllBytes("resample.pcm", data);
}
```

## Todo
- [ ] Streaming methods

## LICENSE

Licensed under GNU GPLv3 with ❤.
