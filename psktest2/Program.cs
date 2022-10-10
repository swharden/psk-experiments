using NAudio.Wave;

namespace Program;

public static class Program
{
    public static void Main()
    {
        int CARRIER_FREQUENCY = 1000;
        int BPSK31_SAMPLE_RATE = 44100;
        double BAUD_RATE = 31.25;
        double HALF_BAUD_RATE = BAUD_RATE / 2;
        bool[] PAYLOAD_BITS = MessageToBitsWithPadding(" TEST1 TEST2 TEST3 TEST4 TEST5 ");
        int[] PAYLOAD_PHASES = BitsToPhases(PAYLOAD_BITS);

        int samples = BPSK31_SAMPLE_RATE * 5;
        double[] outputData = new double[samples];

        for (int i = 0; i < outputData.Length; i++)
        {
            double time = (double)i / BPSK31_SAMPLE_RATE;
            int frame = (int)(time * 1000 / HALF_BAUD_RATE);
            double offset = PAYLOAD_PHASES[frame] == 1 ? Math.PI : 0;
            outputData[i] = Math.Cos(2 * Math.PI * CARRIER_FREQUENCY * time + offset);
        }

        PlaySound(outputData, BPSK31_SAMPLE_RATE);

        ScottPlot.Plot plt = new();
        plt.AddSignal(outputData, BPSK31_SAMPLE_RATE);
        ScottPlot.FormsPlotViewer viewer = new(plt);
        viewer.ShowDialog();
    }

    static void PlaySound(double[] wave, int sampleRate)
    {
        byte[] waveBytes = new byte[wave.Length * 2];
        for (int i = 0; i < wave.Length; i++)
        {
            byte[] values = BitConverter.GetBytes((Int16)(wave[i] * 20));
            waveBytes[i * 2] = values[1];
            waveBytes[i * 2 + 1] = values[0];
        }

        WaveFormat wavFormat = new(sampleRate, 16, 1);
        MemoryStream wavMemoryStream = new(waveBytes);
        RawSourceWaveStream wavStream = new(wavMemoryStream, wavFormat);
        WaveOutEvent output = new();
        output.Init(wavStream);
        output.Play();
    }

    static bool[] MessageToBitsWithPadding(string message)
    {
        // prefix is 11 zeros
        List<bool> bits = new();
        for (int i = 0; i < 11; i++)
        {
            bits.Add(false);
        }

        foreach (char c in message.ToArray())
        {
            bits.AddRange(GetVaricode(c.ToString()));
            bits.Add(false);
            bits.Add(false);
        }

        // prefix is 11 zeros (should be ones?)
        for (int i = 0; i < 11; i++)
        {
            bits.Add(false);
        }

        return bits.ToArray();
    }

    static int[] BitsToPhases(bool[] bits)
    {
        List<int> phases = new() { 0, 1, 1, 0, 0, 1 }; // 000

        foreach (bool bit in bits)
        {
            int previousPhase = phases[phases.Count() - 1];
            int differentPhase = previousPhase == 0 ? 1 : 0;

            if (bit)
            {
                phases.Add(previousPhase);
                phases.Add(previousPhase);
            }
            else
            {
                phases.Add(previousPhase);
                phases.Add(differentPhase);
            }
        }

        return phases.ToArray();
    }

    static bool[] GetVaricode(string letter)
    {
        string code = letter switch
        {
            "NUL" => "1010101011",
            "SOH" => "1011011011",
            "STX" => "1011101101",
            "ETX" => "1101110111",
            "EOT" => "1011101011",
            "ENQ" => "1101011111",
            "ACK" => "1011101111",
            "BEL" => "1011111101",
            "BS" => "1011111111",
            "HT" => "11101111",
            "LF" => "11101",
            "\n" => "11101",
            "VT" => "1101101111",
            "FF" => "1011011101",
            "CR" => "11111",
            "\r" => "11111",
            "SO" => "1101110101",
            "SI" => "1110101011",
            "DLE" => "1011110111",
            "DC1" => "1011110101",
            "DC2" => "1110101101",
            "DC3" => "1110101111",
            "DC4" => "1101011011",
            "NAK" => "1101101011",
            "SYN" => "1101101101",
            "ETB" => "1101010111",
            "CAN" => "1101111011",
            "EM" => "1101111101",
            "SUB" => "1110110111",
            "ESC" => "1101010101",
            "FS" => "1101011101",
            "GS" => "1110111011",
            "RS" => "1011111011",
            "US" => "1101111111",
            "SP" => "1",
            " " => "1",
            "!" => "111111111",
            "\"" => "101011111",
            "#" => "111110101",
            "$" => "111011011",
            "%" => "1011010101",
            "&" => "1010111011",
            "'" => "101111111",
            "(" => "11111011",
            ")" => "11110111",
            "*" => "101101111",
            "+" => "111011111",
            "," => "1110101",
            "-" => "110101",
            "." => "1010111",
            "/" => "110101111",
            "0" => "10110111",
            "1" => "10111101",
            "2" => "11101101",
            "3" => "11111111",
            "4" => "101110111",
            "5" => "101011011",
            "6" => "101101011",
            "7" => "110101101",
            "8" => "110101011",
            "9" => "110110111",
            ":" => "11110101",
            ";" => "110111101",
            "<" => "111101101",
            "=" => "1010101",
            ">" => "111010111",
            "?" => "1010101111",
            "@" => "1010111101",
            "A" => "1111101",
            "B" => "11101011",
            "C" => "10101101",
            "D" => "10110101",
            "E" => "1110111",
            "F" => "11011011",
            "G" => "11111101",
            "H" => "101010101",
            "I" => "1111111",
            "J" => "111111101",
            "K" => "101111101",
            "L" => "11010111",
            "M" => "10111011",
            "N" => "11011101",
            "O" => "10101011",
            "P" => "11010101",
            "Q" => "111011101",
            "R" => "10101111",
            "S" => "1101111",
            "T" => "1101101",
            "U" => "101010111",
            "V" => "110110101",
            "W" => "101011101",
            "X" => "101110101",
            "Y" => "101111011",
            "Z" => "1010101101",
            "[" => "111110111",
            "\\" => "111101111",
            "]" => "111111011",
            "^" => "1010111111",
            "_" => "101101101",
            "`" => "1011011111",
            "a" => "1011",
            "b" => "1011111",
            "c" => "101111",
            "d" => "101101",
            "e" => "11",
            "f" => "111101",
            "g" => "1011011",
            "h" => "101011",
            "i" => "1101",
            "j" => "111101011",
            "k" => "10111111",
            "l" => "11011",
            "m" => "111011",
            "n" => "1111",
            "o" => "111",
            "p" => "111111",
            "q" => "110111111",
            "r" => "10101",
            "s" => "10111",
            "t" => "101",
            "u" => "110111",
            "v" => "1111011",
            "w" => "1101011",
            "x" => "11011111",
            "y" => "1011101",
            "z" => "111010101",
            "" => "1010110111",
            "|" => "110111011",
            //"," => "1010110101",
            "~" => "1011010111",
            "DEL" => "1110110101",
            _ => throw new NotImplementedException(letter)
        };

        return code.ToCharArray().Select(x => x == '1').ToArray();
    }
}