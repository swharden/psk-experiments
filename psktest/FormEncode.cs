using FftSharp;
using NAudio.Wave;
using System;
using System.Diagnostics;

namespace psktest;
public partial class FormEncode : Form
{
    public FormEncode()
    {
        InitializeComponent();
        rtbMessage.Text = "\nThe Quick Brown Fox Jumped Over The Lazy Dog 1234567890 Times!\n";
    }

    private void btnUpdate_Click(object sender, EventArgs e)
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

        WaveFormat wavFormat = new(44100, 16, 1);
        MemoryStream wavMemoryStream = new(waveBytes);
        RawSourceWaveStream wavStream = new(wavMemoryStream, wavFormat);
        WaveOutEvent output = new();
        output.Init(wavStream);
        output.Play();

        formsPlot1.Plot.Clear();
        formsPlot1.Plot.AddSignal(wave, 44100);
        formsPlot1.Refresh();
    }

    private double[] GenerateWavBPSK31(double[] phaseShifts)
    {
        int SampleRate = 44100;
        double carrierFreq = 1000;
        double baudRate = 31.25;
        double transitionRate = baudRate * 2;
        int transitionSamples = (int)(SampleRate / transitionRate);
        int totalSamples = (int)(phaseShifts.Length * SampleRate / transitionRate);
        double[] wave = new double[totalSamples];

        FftSharp.Windows.Cosine cosWindow = new();
        double[] envelope = cosWindow.Create(transitionSamples);

        for (int i = 0; i < wave.Length; i++)
        {
            // phase modulated carrier
            double time = (double)i / SampleRate;
            int frame = (int)(time * transitionRate);
            double phaseShift = phaseShifts[frame];
            wave[i] = Math.Cos(2 * Math.PI * carrierFreq * time + phaseShift);

            // envelope at phase transitions
            int firstSample = (int)(frame * SampleRate / transitionRate);
            int distanceFromFrameStart = i - firstSample;
            int distanceFromFrameEnd = transitionSamples - distanceFromFrameStart + 1;

            if (distanceFromFrameStart < distanceFromFrameEnd)
            {
                if (frame > 0 && phaseShifts[frame - 1] != phaseShifts[frame])
                    wave[i] *= envelope[distanceFromFrameStart];
            }
            else
            {
                if (frame < phaseShifts.Length - 1 && phaseShifts[frame + 1] != phaseShifts[frame])
                    wave[i] *= envelope[distanceFromFrameEnd];
            }
        }

        return wave;
    }
}
