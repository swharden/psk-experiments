using FftSharp;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Policy;

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
        int[] bits = Varicode.GetVaricodeBits(rtbMessage.Text);
        double[] phases = Varicode.GetPhaseShifts(bits);
        double[] wave = GenerateWavBPSK31(phases);

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

    private double[] GenerateWavBPSK31(double[] phases)
    {
        int SampleRate = SAMPLE_RATE;
        double carrierFreq = (double)nudFrequency.Value;
        double baudRate = 31.25; // BPSK31
        //double baudRate = 62.5; // BPSK63
        int baudSamples = (int)(SampleRate / baudRate);
        double samplesPerBit = SampleRate / baudRate;
        int totalSamples = (int)(phases.Length * SampleRate / baudRate);
        double[] wave = new double[totalSamples];

        // create the amplitude envelope sized for a single bit
        double[] envelope = new double[(int)samplesPerBit];
        for (int i = 0; i < envelope.Length; i++)
        {
            envelope[i] = Math.Sin((i + .5) * Math.PI / envelope.Length);
        }

        for (int i = 0; i < wave.Length; i++)
        {
            // phase modulated carrier
            double time = (double)i / SampleRate;
            int frame = (int)(time * baudRate);
            double phaseShift = phases[frame];
            wave[i] = Math.Cos(2 * Math.PI * carrierFreq * time + phaseShift);

            // envelope at phase transitions
            int firstSample = (int)(frame * SampleRate / baudRate);
            int distanceFromFrameStart = i - firstSample;
            int distanceFromFrameEnd = baudSamples - distanceFromFrameStart + 1;
            bool isFirstHalfOfFrame = distanceFromFrameStart < distanceFromFrameEnd;
            bool samePhaseAsLast = frame == 0 ? false : phases[frame - 1] == phases[frame];
            bool samePhaseAsNext = frame == phases.Length - 1 ? false : phases[frame + 1] == phases[frame];
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
