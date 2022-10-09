using NAudio.Dsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
 */

public static class Varicode
{
    private static readonly Dictionary<string, string> CodesBySymbol = new()
    {
        {"NUL", "1010101011"},
        {"SOH", "1011011011"},
        {"STX", "1011101101"},
        { "ETX", "1101110111"},
        { "EOT", "1011101011"},
        { "ENQ", "1101011111"},
        { "ACK", "1011101111"},
        { "BEL", "1011111101"},
        { "BS", "1011111111"},
        { "HT", "11101111"},
        { "LF", "11101"},
        { "VT", "1101101111"},
        { "FF", "1011011101"},
        { "CR", "11111"},
        { "SO", "1101110101"},
        { "SI", "1110101011"},
        { "DLE", "1011110111"},
        { "DC1", "1011110101"},
        { "DC2", "1110101101"},
        { "DC3", "1110101111"},
        { "DC4", "1101011011"},
        { "NAK", "1101101011"},
        { "SYN", "1101101101"},
        { "ETB", "1101010111"},
        { "CAN", "1101111011"},
        { "EM", "1101111101"},
        { "SUB", "1110110111"},
        { "ESC", "1101010101"},
        { "FS", "1101011101"},
        { "GS", "1110111011"},
        { "RS", "1011111011"},
        { "US", "1101111111"},
        { "SP", "1"},
        { "!", "111111111"},
        { "\"", "101011111"},
        { "#", "111110101"},
        { "$", "111011011"},
        { "%", "1011010101"},
        { "&", "1010111011"},
        { "'", "101111111"},
        { "(", "11111011"},
        { ")", "11110111"},
        { "*", "101101111"},
        { "+", "111011111"},
        { ",", "1110101"},
        { "-", "110101"},
        { ".", "1010111"},
        { "/", "110101111"},
        { "0", "10110111"},
        { "1", "10111101"},
        { "2", "11101101"},
        { "3", "11111111"},
        { "4", "101110111"},
        { "5", "101011011"},
        { "6", "101101011"},
        { "7", "110101101"},
        { "8", "110101011"},
        { "9", "110110111"},
        { ":", "11110101"},
        { ";", "110111101"},
        { "<", "111101101"},
        { "=", "1010101"},
        { ">", "111010111"},
        { "?", "1010101111"},
        { "@", "1010111101"},
        { "A", "1111101"},
        { "B", "11101011"},
        { "C", "10101101"},
        { "D", "10110101"},
        { "E", "1110111"},
        { "F", "11011011"},
        { "G", "11111101"},
        { "H", "101010101"},
        { "I", "1111111"},
        { "J", "111111101"},
        { "K", "101111101"},
        { "L", "11010111"},
        { "M", "10111011"},
        { "N", "11011101"},
        { "O", "10101011"},
        { "P", "11010101"},
        { "Q", "111011101"},
        { "R", "10101111"},
        { "S", "1101111"},
        { "T", "1101101"},
        { "U", "101010111"},
        { "V", "110110101"},
        { "W", "101011101"},
        { "X", "101110101"},
        { "Y", "101111011"},
        { "Z", "1010101101"},
        { "[", "111110111"},
        { "\\", "111101111"},
        { "]", "111111011"},
        { "^", "1010111111"},
        { "_", "101101101"},
        { "`", "1011011111"},
        { "a", "1011"},
        { "b", "1011111"},
        { "c", "101111"},
        { "d", "101101"},
        { "e", "11"},
        { "f", "111101"},
        { "g", "1011011"},
        { "h", "101011"},
        { "i", "1101"},
        { "j", "111101011"},
        { "k", "10111111"},
        { "l", "11011"},
        { "m", "111011"},
        { "n", "1111"},
        { "o", "111"},
        { "p", "111111"},
        { "q", "110111111"},
        { "r", "10101"},
        { "s", "10111"},
        { "t", "101"},
        { "u", "110111"},
        { "v", "1111011"},
        { "w", "1101011"},
        { "x", "11011111"},
        { "y", "1011101"},
        { "z", "111010101"},
        { "{", "1010110111"},
        { "|", "110111011"},
        { "},", "1010110101"},
        { "~", "1011010111"},
        { "DEL", "1110110101" }
    };

    public static Dictionary<string, string> GetCodesBySymbol() => CodesBySymbol;

    public static string GetCode(string symbol)
    {
        CodesBySymbol.TryGetValue(symbol, out string? code);
        return code ?? "ERROR";
    }

    public static string GetSymbol(string code)
    {
        if (code.Replace("1", "").Replace("0", "").Length > 0)
            throw new InvalidOperationException("code must only contain 1 and 0");

        foreach (var p in CodesBySymbol)
        {
            if (p.Value == code)
                return p.Key;
        }

        return "ERROR";
    }

    public static string GetSymbols(string codeblock)
    {
        StringBuilder sb = new();
        foreach (string symbolCode in codeblock.Split("00"))
        {
            string symbol = GetSymbol(symbolCode);
            if (symbol == "ERROR")
                continue;
            if (symbol == "SOH")
                continue;
            if (symbol == "CR")
                symbol = "\r";
            if (symbol == "LF")
                symbol = "\n";
            if (symbol == "SP")
                symbol = " ";
            sb.Append(symbol);
        }

        return sb.ToString();
    }
}
