namespace psktest;

public struct VaricodeSymbol
{
    /// <summary>
    /// Symbol (e.g., a space is "SP")
    /// </summary>
    public readonly string Symbol;

    /// <summary>
    /// Variable length binary code for this symbol
    /// </summary>
    public string BitString;

    /// <summary>
    /// Variable length array of bit values for this symbol
    /// </summary>
    public int[] Bits;

    /// <summary>
    /// Additional information about this symbol
    /// </summary>
    public readonly string Description;

    public VaricodeSymbol(string symbol, string bitString, string? description = null)
    {
        if (bitString.Replace("1", "").Replace("0", "").Length > 0)
            throw new ArgumentException($"{nameof(bitString)} must only contain 1 and 0");

        if (bitString.Contains("00"))
            throw new InvalidOperationException("Varicode bits must not contain consecutive zeros");

        Symbol = symbol;
        BitString = bitString;
        Bits = bitString.ToCharArray().Select(x => x == '1' ? 1 : 0).ToArray();
        Description = description ?? string.Empty;
    }
}
