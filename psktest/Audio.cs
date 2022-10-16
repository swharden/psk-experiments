using Microsoft.VisualBasic;
using NAudio.Wave;

namespace psktest;
public static class Audio
{
    /// <summary>
    /// Play a single-channel wave using the sound card
    /// </summary>
    public static void Play(double[] wave, int sampleRate)
    {
        byte[] waveBytes = EncodeWave(wave);
        WaveFormat wavFormat = new(sampleRate, 16, 1);
        MemoryStream wavMemoryStream = new(waveBytes);
        RawSourceWaveStream wavStream = new(wavMemoryStream, wavFormat);
        WaveOutEvent output = new();
        output.Init(wavStream);
        output.Play();
    }

    /// <summary>
    /// Return bytes for a WAV file from floating point audio levels
    /// </summary>
    public static byte[] EncodeWave(double[] wave)
    {
        byte[] bytes = new byte[wave.Length * 2];
        for (int i = 0; i < wave.Length; i++)
        {
            byte[] values = BitConverter.GetBytes((Int16)(wave[i] * 30));
            bytes[i * 2] = values[1];
            bytes[i * 2 + 1] = values[0];
        }
        return bytes;
    }

    public static void CreateWavFile(double[] wave, string saveAs, int sampleRate, int bits = 16, int channels = 1)
    {
        WaveFormat waveFormat = new(sampleRate, bits, channels);
        using WaveFileWriter writer = new(saveAs, waveFormat);
        byte[] bytes = EncodeWave(wave);
        writer.Write(bytes, 0, bytes.Length);
    }
}
