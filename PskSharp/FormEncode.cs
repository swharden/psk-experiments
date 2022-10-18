using FftSharp;
using Microsoft.VisualBasic.Devices;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Policy;

namespace psktest;
public partial class FormEncode : Form
{
    const int SAMPLE_RATE = 8000;

    public FormEncode()
    {
        InitializeComponent();
        rtbMessage.Text = "\nThe Quick Brown Fox Jumped Over The Lazy Dog 1234567890 Times!\n";
        formsPlot1.Plot.Style(figureBackground: Color.White);
        Generate();
    }

    private void btnUpdate_Click(object sender, EventArgs e) => Generate();

    private double[] Generate()
    {
        PskWaveform psk = new()
        {
            Frequency = (double)nudFrequency.Value,
            BaudRate = double.Parse(cbBaudRate.Text),
            SampleRate = SAMPLE_RATE,
        };

        int[] bits = Varicode.GetVaricodeBits(rtbMessage.Text);
        rtbSymbols.Text = string.Join("", bits.Select(x => x.ToString()));

        double[] phases = Varicode.GetPhaseShifts(bits);
        double[] wave = psk.GetWaveformBPSK(phases, cbEnvelope.Checked);
        UpdatePlot(wave);

        return wave;
    }

    private void btnPlay_Click(object sender, EventArgs e)
    {
        double[] wave = Generate();
        Audio.Play(wave, SAMPLE_RATE);
    }

    private void UpdatePlot(double[] wave)
    {
        formsPlot1.Plot.Clear();
        formsPlot1.Plot.AddSignal(wave, SAMPLE_RATE);
        formsPlot1.Refresh();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        double[] wave = Generate();

        SaveFileDialog savefile = new SaveFileDialog();
        savefile.FileName = "psk.wav";
        savefile.Filter = "WAV Files (*.wav)|*.wav";
        if (savefile.ShowDialog() == DialogResult.OK)
        {
            Audio.CreateWavFile(wave, savefile.FileName, SAMPLE_RATE);
        }
    }
}
