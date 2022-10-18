using FftSharp;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace psktest;

public partial class FormDecode : Form
{
    public readonly double[] AudioSource;
    public readonly double SampleRate;

    public FormDecode()
    {
        InitializeComponent();
        string path = "../../../../data/sample-bpsk31-8khz.wav";
        (AudioSource, SampleRate) = ReadWAV(path, 1);
        //AudioSource = AudioSource.Take(20_000).ToArray();

        UpdatePlot();
        formsPlot1.Plot.SetAxisLimits(0, 1.5, -.5, .5);
    }

    private void trackBar1_Scroll(object sender, EventArgs e) => UpdatePlot();
    private void trackBar2_Scroll(object sender, EventArgs e) => UpdatePlot();

    public void UpdatePlot()
    {
        double SymbolsPerSecond = 31.25;
        double SymbolTime = 1.0 / SymbolsPerSecond;

        int phaseOffset = trackBar1.Value + 8;
        int diffPoints = (int)(SymbolTime * SampleRate);
        double[] diff = new double[AudioSource.Length - diffPoints - phaseOffset];
        for (int i = 0; i < diff.Length; i++)
        {
            diff[i] = AudioSource[phaseOffset + i + diffPoints] * AudioSource[i];
        }

        int lpfPoints = 25;
        double[] smoothDiff = new double[diff.Length - lpfPoints];
        for (int i = 0; i < smoothDiff.Length; i++)
        {
            smoothDiff[i] = diff.Skip(i).Take(lpfPoints).Sum() / lpfPoints;
        }

        var originalLimits = formsPlot1.Plot.GetAxisLimits();
        bool preserveOriginalLimits = formsPlot1.Plot.GetPlottables().Any();
        formsPlot1.Plot.Clear();
        formsPlot1.Plot.AddSignal(smoothDiff, SampleRate);

        List<int> bits = new();
        double firstMeasurement = .1355;
        int charsDecoded = 0;
        while (true)
        {
            double sampleTime = firstMeasurement + charsDecoded * SymbolTime;
            int sampleIndex = (int)(sampleTime * SampleRate);
            if (sampleIndex >= smoothDiff.Length)
                break;
            charsDecoded += 1;

            bool isHigh = smoothDiff[sampleIndex] > 0;
            Color color = isHigh ? Color.DarkGray : Color.Gray;
            bits.Add(isHigh ? 1 : 0);

            formsPlot1.Plot.AddVerticalLine(sampleTime, color, 2);
            var t = formsPlot1.Plot.AddText(isHigh ? "1" : "0", sampleTime, .35);
            t.Alignment = ScottPlot.Alignment.MiddleCenter;
            t.BackgroundFill = true;
            t.BackgroundColor = color;
            t.FontBold = true;
            t.FontSize = 12;
            t.Color = Color.Black;
        }

        if (preserveOriginalLimits)
            formsPlot1.Plot.SetAxisLimits(originalLimits);

        formsPlot1.Plot.Grid(false);

        formsPlot1.Refresh();
        richTextBox1.Text = Varicode.DecodeSymbolBitsToText(bits);
        richTextBox2.Text = string.Join("", bits.Select(x=>x.ToString()));
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
