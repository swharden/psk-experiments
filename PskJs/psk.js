/* PSK-31 Audio Encoder by Scott Harden
 * License: MIT https://opensource.org/licenses/MIT
 * GitHub: https://github.com/swharden/psk-experiments
 * Blog Post: https://swharden.com/blog/2022-10-16-psk31-synthesis
 * Extends work by Jacob Gillespie: https://github.com/jacobwgillespie/psk31
 */

function getPskWaveform(sampleRate, freq, baud, message, envelope) {

    const volume = .5;
    const varicodeBits = getVaricodeBits(message);
    const phases = getPhaseShifts(varicodeBits);

    const samplesPerSymbol = Math.floor(sampleRate / baud);
    const symbolWindow = Array.from(Array(samplesPerSymbol).keys()).map(x => Math.sin(x / samplesPerSymbol * Math.PI));

    const waveform = new Float32Array(phases.length * sampleRate / baud);

    for (var i = 0; i < waveform.length; i++) {
        const symbolIndex = Math.floor(i * baud / sampleRate);
        waveform[i] = Math.sin(i * freq / sampleRate * 2 * Math.PI + phases[symbolIndex]) * volume;
        if (envelope == false)
            continue;

        const symbolStartIndex = Math.floor(symbolIndex * sampleRate / baud);
        const samplesIntoSymbol = i - symbolStartIndex;
        const isFirstHalfOfSymbol = samplesIntoSymbol < samplesPerSymbol / 2;
        if (isFirstHalfOfSymbol) {
            const phaseDifferentFromLast = symbolIndex > 0 && phases[symbolIndex] != symbolIndex[symbolIndex - 1];
            if (phaseDifferentFromLast) {
                waveform[i] *= symbolWindow[samplesIntoSymbol];
            }
        } else {
            const phaseDifferentFromNext = (symbolIndex < phases.length - 1) && (phases[symbolIndex] != symbolIndex[symbolIndex + 1]);
            if (phaseDifferentFromNext) {
                const samplesFromEnd = samplesPerSymbol - samplesIntoSymbol;
                waveform[i] *= symbolWindow[symbolWindow.length - 1 - samplesFromEnd];
            }
        }
    }

    return waveform;
}

function getVaricodeBits(message) {
    const bits = new Array();

    // preamble
    for (var i = 0; i < 25; i++) {
        bits.push(0);
    }

    // bits for each letter separated by two zeros
    message.split('').forEach(character => {
        VARICODE[character].forEach(bit => bits.push(bit))
        bits.push(0);
        bits.push(0);
    });

    // postamble
    for (var i = 0; i < 25; i++) {
        bits.push(1);
    }

    console.log("Varicode Bits:", bits);
    return bits;
}

function getPhaseShifts(varicodeBits) {
    const phases = new Array();
    phases.push(0);

    for (var i = 1; i < varicodeBits.length; i++) {
        const lastPhase = phases[i - 1];
        const nextPhase = varicodeBits[i] == 0 ? (lastPhase + 180) % 360 : lastPhase
        phases.push(nextPhase);
    }

    console.log("Phases:", varicodeBits);
    return phases;
}

const VARICODE = {
    '\x00': 1010101011, // NUL
    '\x01': 1011011011, // SOH
    '\x02': 1011101101, // STX
    '\x03': 1101110111, // ETX
    '\x04': 1011101011, // EOT
    '\x05': 1101011111, // ENQ
    '\x06': 1011101111, // ACK
    '\x07': 1011111101, // BEL
    '\x08': 1011111111, // BS
    '\x09': 11101111, // HT
    '\x0a': 11101, // LF
    '\x0b': 1101101111, // VT
    '\x0c': 1011011101, // FF
    '\x0d': 11111, // CR
    '\x0e': 1101110101, // SO
    '\x0f': 1110101011, // SI
    '\x10': 1011110111, // DLE
    '\x11': 1011110101, // DC1
    '\x12': 1110101101, // DC2
    '\x13': 1110101111, // DC3
    '\x14': 1101011011, // DC4
    '\x15': 1101101011, // NAK
    '\x16': 1101101101, // SYN
    '\x17': 1101010111, // ETB
    '\x18': 1101111011, // CAN
    '\x19': 1101111101, // EM
    '\x1a': 1110110111, // SUB
    '\x1b': 1101010101, // ESC
    '\x1c': 1101011101, // FS
    '\x1d': 1110111011, // GS
    '\x1e': 1011111011, // RS
    '\x1f': 1101111111, // US
    ' ': 1,
    '!': 111111111,
    '"': 101011111,
    '#': 111110101,
    $: 111011011,
    '%': 1011010101,
    '&': 1010111011,
    "'": 101111111,
    '(': 11111011,
    ')': 11110111,
    '*': 101101111,
    '+': 111011111,
    ',': 1110101,
    '-': 110101,
    '.': 1010111,
    '/': 110101111,
    0: 10110111,
    1: 10111101,
    2: 11101101,
    3: 11111111,
    4: 101110111,
    5: 101011011,
    6: 101101011,
    7: 110101101,
    8: 110101011,
    9: 110110111,
    ':': 11110101,
    ';': 110111101,
    '<': 111101101,
    '=': 1010101,
    '>': 111010111,
    '?': 1010101111,
    '@': 1010111101,
    A: 1111101,
    B: 11101011,
    C: 10101101,
    D: 10110101,
    E: 1110111,
    F: 11011011,
    G: 11111101,
    H: 101010101,
    I: 1111111,
    J: 111111101,
    K: 101111101,
    L: 11010111,
    M: 10111011,
    N: 11011101,
    O: 10101011,
    P: 11010101,
    Q: 111011101,
    R: 10101111,
    S: 1101111,
    T: 1101101,
    U: 101010111,
    V: 110110101,
    W: 101011101,
    X: 101110101,
    Y: 101111011,
    Z: 1010101101,
    '[': 111110111,
    '\\': 111101111,
    ']': 111111011,
    '^': 1010111111,
    _: 101101101,
    '`': 1011011111,
    a: 1011,
    b: 1011111,
    c: 101111,
    d: 101101,
    e: 11,
    f: 111101,
    g: 1011011,
    h: 101011,
    i: 1101,
    j: 111101011,
    k: 10111111,
    l: 11011,
    m: 111011,
    n: 1111,
    o: 111,
    p: 111111,
    q: 110111111,
    r: 10101,
    s: 10111,
    t: 101,
    u: 110111,
    v: 1111011,
    w: 1101011,
    x: 11011111,
    y: 1011101,
    z: 111010101,
    '{': 1010110111,
    '|': 110111011,
    '}': 1010110101,
    '~': 1011010111,
    '\x7F': 1110110101, // DEL
}

Object.keys(VARICODE).forEach(character => {
    VARICODE[character] = VARICODE[character]
        .toString(10)
        .split('')
        .map(c => parseInt(c, 10))
})