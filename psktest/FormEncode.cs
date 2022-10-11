using FftSharp;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;

namespace psktest;
public partial class FormEncode : Form
{
    const int SAMPLE_RATE = 8_000;

    public FormEncode()
    {
        InitializeComponent();
        rtbMessage.Text = "\nThe Quick Brown Fox Jumped Over The Lazy Dog 1234567890 Times!\n";
        Generate(false);
    }

    private void btnUpdate_Click(object sender, EventArgs e) => Generate(false);

    private void btnPlay_Click(object sender, EventArgs e) => Generate(true);

    private void Generate(bool play)
    {
        double[] phaseBits = Varicode.GetBinaryPhaseShifts(rtbMessage.Text);
        double[] wave = GenerateWavBPSK31(phaseBits);

        byte[] waveBytes = new byte[wave.Length * 2];
        for (int i = 0; i < wave.Length; i++)
        {
            byte[] values = BitConverter.GetBytes((Int16)(wave[i] * 30));
            waveBytes[i * 2] = values[1];
            waveBytes[i * 2 + 1] = values[0];
        }

        formsPlot1.Plot.Clear();
        formsPlot1.Plot.AddSignal(wave, SAMPLE_RATE);
        formsPlot1.Refresh();

        if (play)
        {
            WaveFormat wavFormat = new(SAMPLE_RATE, 16, 1);
            MemoryStream wavMemoryStream = new(waveBytes);
            RawSourceWaveStream wavStream = new(wavMemoryStream, wavFormat);
            WaveOutEvent output = new();
            output.Init(wavStream);
            output.Play();
        }
    }

    private double[] CosineWindow(int size)
    {
        return Enumerable.Range(0, size).Select(x => Math.Sin(Math.PI / (size) * (x + .5))).ToArray();
    }

    private double[] GenerateWavBPSK31(double[] phaseShifts)
    {
        int SampleRate = SAMPLE_RATE;
        double carrierFreq = 2000;
        double baudRate = 31.25;
        int baudSamples = (int)(SampleRate / baudRate);
        double samplesPerBit = SampleRate / baudRate;
        int totalSamples = (int)(phaseShifts.Length * SampleRate / baudRate);
        double[] wave = new double[totalSamples];

        double[] envelope = CosineWindow((int)samplesPerBit);

        for (int i = 0; i < wave.Length; i++)
        {
            // phase modulated carrier
            double time = (double)i / SampleRate;
            int frame = (int)(time * baudRate);
            double phaseShift = phaseShifts[frame];
            wave[i] = Math.Cos(2 * Math.PI * carrierFreq * time + phaseShift);

            // envelope at phase transitions
            int firstSample = (int)(frame * SampleRate / baudRate);
            int distanceFromFrameStart = i - firstSample;
            int distanceFromFrameEnd = baudSamples - distanceFromFrameStart + 1;
            bool isFirstHalfOfFrame = distanceFromFrameStart < distanceFromFrameEnd;
            bool samePhaseAsLast = frame == 0 ? false : phaseShifts[frame - 1] == phaseShifts[frame];
            bool samePhaseAsNext = frame == phaseShifts.Length - 1 ? false : phaseShifts[frame + 1] == phaseShifts[frame];
            bool rampUp = isFirstHalfOfFrame && !samePhaseAsLast;
            bool rampDown = !isFirstHalfOfFrame && !samePhaseAsNext;

            if (rampUp)
                wave[i] *= envelope[distanceFromFrameStart];

            if (rampDown)
                wave[i] *= envelope[distanceFromFrameEnd];
        }

        return wave;
    }
}
