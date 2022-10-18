namespace psktest;
public class PskWaveform
{
    /// <summary>
    /// Carrier frequency for the encoded PSK message.
    /// For ideal phase encoding the sample rate should be at least five times this value.
    /// </summary>
    public double Frequency = 500;

    /// <summary>
    /// Rate that varicode bits are transmitted.
    /// This value is typically 31.25 (PSK31), 62.5 (PSK63), 125, or 250
    /// </summary>
    public double BaudRate = 31.25;

    /// <summary>
    /// Sample rate of the encoded waveform
    /// </summary>
    public int SampleRate = 8_000;

    public PskWaveform()
    {

    }

    public double[] GetWaveformBPSK(double[] phases, bool applyEnvelope)
    {
        int baudSamples = (int)(SampleRate / BaudRate);
        double samplesPerBit = SampleRate / BaudRate;
        int totalSamples = (int)(phases.Length * SampleRate / BaudRate);
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
            int frame = (int)(time * BaudRate);
            double phaseShift = phases[frame];
            wave[i] = Math.Cos(2 * Math.PI * Frequency * time + phaseShift);

            if (applyEnvelope)
            {
                // envelope at phase transitions
                int firstSample = (int)(frame * SampleRate / BaudRate);
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
        }

        return wave;
    }
}
