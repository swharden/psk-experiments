using FftSharp;
using System.Diagnostics;
using System.Drawing;
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
        //AudioSource = AudioSource.Take(20_000).ToArray();

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

        int averageSize = 25;
        double[] smoothedCos = new double[mixedCos.Length - averageSize];
        double[] smoothedSin = new double[mixedCos.Length - averageSize];
        for (int i = 0; i < smoothedCos.Length; i++)
        {
            smoothedCos[i] = mixedCos.Skip(i).Take(averageSize).Sum() / averageSize;
            smoothedSin[i] = mixedSin.Skip(i).Take(averageSize).Sum() / averageSize;
        }

        double[] smoothedSum = new double[smoothedCos.Length];
        for (int i = 0; i < smoothedSum.Length; i++)
        {
            smoothedSum[i] = smoothedCos[i] + smoothedSin[i];
        }

        var originalLimits = formsPlot1.Plot.GetAxisLimits();
        bool preserveOriginalLimits = formsPlot1.Plot.GetPlottables().Any();
        formsPlot1.Plot.Clear();

        //formsPlot1.Plot.AddSignal(smoothedCos, SampleRate);
        //formsPlot1.Plot.AddSignal(smoothedSin, SampleRate);
        formsPlot1.Plot.AddSignal(smoothedSum, SampleRate);
        

        double firstSymbolOffset = .105 + SymbolTime;
        for (int symbolNumber = 0; symbolNumber < 1000; symbolNumber++)
        {
            double bitStartTime = symbolNumber * SymbolTime + firstSymbolOffset;
            if (bitStartTime * SampleRate > smoothedCos.Length)
                break;

            int i = (int)(bitStartTime * SampleRate);
            bool isHigh = smoothedCos[i] > smoothedSin[i];

            //if (isHigh)
                //formsPlot1.Plot.AddVerticalLine(bitStartTime, Color.Black, 2);

            /*
            Color color = isHigh ? Color.Black : Color.Gray;
            float thickness = isHigh ? 3 : 1;
            formsPlot1.Plot.AddVerticalLine(bitStartTime, color, thickness);
            */
        }

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
