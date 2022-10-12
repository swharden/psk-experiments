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
        PskWaveform psk = new()
        {
            Frequency = (double)nudFrequency.Value,
            BaudRate = double.Parse(cbBaudRate.Text),
            SampleRate = 8_000,
        };

        int[] bits = Varicode.GetVaricodeBits(rtbMessage.Text);
        rtbSymbols.Text = string.Join("", bits.Select(x => x.ToString()));

        double[] phases = Varicode.GetPhaseShifts(bits);
        double[] wave = psk.GetWaveformBPSK(phases);

        formsPlot1.Plot.Clear();
        formsPlot1.Plot.AddSignal(wave, psk.SampleRate);
        formsPlot1.Refresh();

        if (play)
        {
            Audio.Play(wave, psk.SampleRate);
        }
    }

}
