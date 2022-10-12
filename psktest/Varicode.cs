namespace psktest;

/* Varicode Encoding/Decoding Routines 
 *   Copyright 2022 Scott W Harden, AJ4VD
 *   Released under the MIT license
 *   https://opensource.org/licenses/MIT
 *   
 * Resources
 *   http://www.arrl.org/psk31-spec
 *   http://math0.wvstateu.edu/~baker/cs240/info/varicode.html
 *   http://aintel.bi.ehu.es/psk31theory.html
 *   https://n7yg.net/psk-stuff/psk31-varicode
 *   http://aintel.bi.ehu.es/psk31.html
 *   https://sites.google.com/site/psk31matlabproject/introduction-to-psk
 *   https://sdradventure.wordpress.com/2011/10/15/gnuradio-psk31-decoder-part-1/
 *   https://github.dev/marcino239/psk31/blob/master/psk31_receiver.ipynb ←NICE
 *   https://github.com/ghostop14/psk31
 *   https://pysdr.org/content/digital_modulation.html#phase-shift-keying-psk
 *   https://myplace.frontier.com/~nb6z/psk31.htm
 *   
 * Varicode Encoding Rules:
 *   1. Each character is represented by a variable length series of of bits (varicode)
 *   2. Character bits must start and end with 1
 *   3. Character bits must never contain consecutive 0s
 *   4. Two consecutive 0s represent the character separator
 *   5. Messages have a preamble of consecutive 1s
 *   6. Pauses between characters may be encoded as an arbitrary number of consecutive zeros
 *   
 * Notes:
 *   Since a 1 is encoded as a phase transition, the preamble (many consecutive 1s) is continuous phase
 *   transitions at the baud rate (~30 Hz) which behaves like a 15 Hz sine wave. When these two waves
 *   are mixed it produces two tones +/- ~15Hz on each side of the carrier. This is why the
 *   preamble on the spectrogram appears as a pair of tones. Conversely, the repeated zeros from the
 *   postamble (or the consecutive zeros sent during spaces between characters) are encoded as the carrier
 *   without phase transitions, and on the spectrogram appears as a single tone.
 * 
 */

public static class Varicode
{
    public static readonly VaricodeSymbol[] Symbols = GetAllSymbols();

    public static int[] Preamble = Enumerable.Range(0, 25).Select(x => 0).ToArray();
    public static int[] Postamble = Enumerable.Range(0, 25).Select(x => 1).ToArray();
    public static int[] CharacterSeparator = Enumerable.Range(0, 2).Select(x => 0).ToArray();

    /// <summary>
    /// Generate bits of a varicode message from the given string
    /// </summary>
    public static int[] GetVaricodeBits(string message)
    {
        List<int> bits = new();

        bits.AddRange(Preamble);

        foreach (char character in message)
        {
            VaricodeSymbol symbol = Lookup(character);
            bits.AddRange(symbol.Bits);
            bits.AddRange(CharacterSeparator);
        }

        bits.AddRange(Postamble);

        return bits.ToArray();
    }

    /// <summary>
    /// Generate phase shifts for binary phase shift keying (BPSK) from bits of a varicode message
    /// </summary>
    public static double[] GetPhaseShifts(int[] bits, double phase1 = 0, double phase2 = Math.PI)
    {
        double[] phases = new double[bits.Length];

        for (int i = 0; i < bits.Length; i++)
        {
            double previousPhase = i > 0 ? phases[i - 1] : phase1;
            double oppositePhase = previousPhase == phase1 ? phase2 : phase1;
            phases[i] = bits[i] == 1 ? previousPhase : oppositePhase;
        }

        return phases;
    }

    private static VaricodeSymbol Lookup(char c)
    {
        string symbol = c.ToString() switch
        {
            " " => "SP",
            "\r" => "CR",
            "\n" => "LF",
            _ => c.ToString(),
        };

        foreach (VaricodeSymbol s in GetAllSymbols())
        {
            if (symbol == s.Symbol)
                return s;
        }

        return new VaricodeSymbol("?", "", $"Unsupported character: '{symbol}'");
    }

    public static string DecodeSymbolBitsToText(IList<int> bits)
    {
        return "not impl";
    }

    private static VaricodeSymbol[] GetAllSymbols() => new VaricodeSymbol[]
    {
        new("NUL", "1010101011", "Null character"),
        new("SOH", "1011011011", "Start of Header"),
        new("STX", "1011101101", "Start of Text"),
        new("ETX", "1101110111", "End of Text"),
        new("EOT", "1011101011", "End of Transmission"),
        new("ENQ", "1101011111", " Enquiry"),
        new("ACK", "1011101111", "Acknowledgment"),
        new("BEL", "1011111101", "Bell"),
        new("BS", "1011111111", "Backspace"),
        new("HT", "11101111", "Horizontal Tab"),
        new("LF", "11101", "Line feed"),
        new("VT", "1101101111", "Vertical Tab"),
        new("FF", "1011011101", "Form feed"),
        new("CR", "11111", "Carriage return"),
        new("SO", "1101110101", "Shift Out"),
        new("SI", "1110101011", "Shift In"),
        new("DLE", "1011110111", "Data Link Escape"),
        new("DC1", "1011110101", "Device Control 1 (XON)"),
        new("DC2", "1110101101", "Device Control 2"),
        new("DC3", "1110101111", "Device Control 3 (XOFF)"),
        new("DC4", "1101011011", "Device Control 4"),
        new("NAK", "1101101011", "Negative Acknowledgement"),
        new("SYN", "1101101101", "Synchronous Idle"),
        new("ETB", "1101010111", "End of Trans. Block"),
        new("CAN", "1101111011", "Cancel character|Cancel"),
        new("EM", "1101111101", "End of Medium"),
        new("SUB", "1110110111", "Substitute"),
        new("ESC", "1101010101", "Escape"),
        new("FS", "1101011101", "File Separator"),
        new("GS", "1110111011", "Group Separator"),
        new("RS", "1011111011", "Record Separator"),
        new("US", "1101111111", "Unit Separator"),
        new("DEL", "1110110101", "Delete"),
        new("SP", "1", "Space"),
        new("!", "111111111", "Exclaimation Mark"),
        new("\"", "101011111", "Qutation Mark"),
        new("#", "111110101"),
        new("$", "111011011"),
        new("%", "1011010101"),
        new("&", "1010111011"),
        new("'", "101111111"),
        new("(", "11111011"),
        new(")", "11110111"),
        new("*", "101101111"),
        new("+", "111011111"),
        new(",", "1110101"),
        new("-", "110101"),
        new(".", "1010111"),
        new("/", "110101111"),
        new("0", "10110111"),
        new("1", "10111101"),
        new("2", "11101101"),
        new("3", "11111111"),
        new("4", "101110111"),
        new("5", "101011011"),
        new("6", "101101011"),
        new("7", "110101101"),
        new("8", "110101011"),
        new("9", "110110111"),
        new(":", "11110101"),
        new(";", "110111101"),
        new("<", "111101101"),
        new("=", "1010101"),
        new(">", "111010111"),
        new("?", "1010101111"),
        new("@", "1010111101"),
        new("A", "1111101"),
        new("B", "11101011"),
        new("C", "10101101"),
        new("D", "10110101"),
        new("E", "1110111"),
        new("F", "11011011"),
        new("G", "11111101"),
        new("H", "101010101"),
        new("I", "1111111"),
        new("J", "111111101"),
        new("K", "101111101"),
        new("L", "11010111"),
        new("M", "10111011"),
        new("N", "11011101"),
        new("O", "10101011"),
        new("P", "11010101"),
        new("Q", "111011101"),
        new("R", "10101111"),
        new("S", "1101111"),
        new("T", "1101101"),
        new("U", "101010111"),
        new("V", "110110101"),
        new("W", "101011101"),
        new("X", "101110101"),
        new("Y", "101111011"),
        new("Z", "1010101101"),
        new("[", "111110111"),
        new("\\", "111101111"),
        new("]", "111111011"),
        new("^", "1010111111"),
        new("_", "101101101"),
        new("'", "1011011111"),
        new("a", "1011"),
        new("b", "1011111"),
        new("c", "101111"),
        new("d", "101101"),
        new("e", "11"),
        new("f", "111101"),
        new("g", "1011011"),
        new("h", "101011"),
        new("i", "1101"),
        new("j", "111101011"),
        new("k", "10111111"),
        new("l", "11011"),
        new("m", "111011"),
        new("n", "1111"),
        new("o", "111"),
        new("p", "111111"),
        new("q", "110111111"),
        new("r", "10101"),
        new("s", "10111"),
        new("t", "101"),
        new("u", "110111"),
        new("v", "1111011"),
        new("w", "1101011"),
        new("x", "11011111"),
        new("y", "1011101"),
        new("z", "111010101"),
        new("{", "1010110111"),
        new("|", "110111011"),
        new("}", "1010110101"),
        new("~", "1011010111"),
    };
}
