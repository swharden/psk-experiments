using FftSharp;
using System.Diagnostics;
using System.IO;

namespace psktest;

public partial class Form1 : Form
{
    public readonly double[] AudioSource;
    public readonly double SampleRate;

    public Form1()
    {
        InitializeComponent();
        string path = "../../../../data/sample-bpsk31-8khz.wav";
        (AudioSource, SampleRate) = ReadWAV(path, 1);
        AudioSource = AudioSource.Take(20_000).ToArray();

        UpdatePlot();
    }

    private void trackBar1_Scroll(object sender, EventArgs e) => UpdatePlot();
    private void trackBar2_Scroll(object sender, EventArgs e) => UpdatePlot();

    public void UpdatePlot()
    {
        double SymbolsPerSecond = 31.25;
        double SymbolTime = 1.0 / SymbolsPerSecond;

        double[] CarrierCos = new double[AudioSource.Length];
        double[] CarrierSin = new double[AudioSource.Length];
        double CarrierFreq = 1000;

        double CarrierOffsetFraction = (double)trackBar1.Value / 100;
        double CarrierOffsetTime = CarrierOffsetFraction * SymbolTime * .03;
        label1.Text = $"Carrier Phase Shift: {CarrierOffsetTime * 1000:N3} ms";

        for (int i = 0; i < CarrierCos.Length; i++)
        {
            double time = i / SampleRate + CarrierOffsetTime;
            CarrierCos[i] = Math.Cos(time * Math.PI * 2 * CarrierFreq);
            CarrierSin[i] = Math.Sin(time * Math.PI * 2 * CarrierFreq);
        }

        double[] mixedCos = new double[AudioSource.Length];
        double[] mixedSin = new double[AudioSource.Length];
        for (int i = 0; i < AudioSource.Length; i++)
        {
            mixedCos[i] = AudioSource[i] * CarrierCos[i];
            mixedSin[i] = AudioSource[i] * CarrierSin[i];
        }

        //PlotFft(mixedCos);
        //PlotFft(mixedSin);

        var originalLimits = formsPlot1.Plot.GetAxisLimits();
        bool preserveOriginalLimits = formsPlot1.Plot.GetPlottables().Any();
        formsPlot1.Plot.Clear();
        formsPlot1.Plot.AddSignal(mixedCos);
        formsPlot1.Plot.AddSignal(mixedSin);
        if (preserveOriginalLimits)
            formsPlot1.Plot.SetAxisLimits(originalLimits);
        formsPlot1.Refresh();
    }

    public void PlotFft(double[] values)
    {
        values = FftSharp.Pad.ZeroPad(values);
        FftSharp.Windows.Hanning window = new();
        window.ApplyInPlace(values);
        Complex[] fft = FftSharp.Transform.FFT(values);
        double[] fftReal = fft.Select(x => x.Real).ToArray();
        double[] fftImag = fft.Select(x => x.Imaginary).ToArray();
        double[] fftFreq = FftSharp.Transform.FFTfreq(SampleRate, fft.Length, false);
        formsPlot1.Plot.AddSignal(fftReal, 1.0 / fftFreq[1]);
    }

    public (double[] audio, int sampleRate) ReadWAV(string filePath, double multiplier = 16_000)
    {
        using NAudio.Wave.AudioFileReader afr = new(filePath);
        int sampleRate = afr.WaveFormat.SampleRate;
        int sampleCount = (int)(afr.Length / afr.WaveFormat.BitsPerSample / 8);
        int channelCount = afr.WaveFormat.Channels;
        var audio = new List<double>(sampleCount);
        var buffer = new float[sampleRate * channelCount];
        int samplesRead = 0;
        while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
            audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
        return (audio.ToArray(), sampleRate);
    }
}
