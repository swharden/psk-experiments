"""
Experiments in decoding QPSK31
"""

import numpy as np
from scipy.io import wavfile
import matplotlib.pyplot as plt


def showFft(sampleRate: float, data: np.ndarray):
    fft = np.fft.fft(data)
    freq = np.fft.fftfreq(len(data), 1.0/sampleRate)
    plt.plot(freq, fft.real, freq, fft.imag)
    plt.show()


def showWav(sampleRate: float, data: np.ndarray):
    times = np.arange(len(data)) / sampleRate
    plt.plot(times, data)
    plt.show()


def decode(sampleRate: float, data: np.ndarray):
    # https://pysdr.org/content/rds.html
    baud = 31.25
    symbolLengthSec = 1.0 / baud
    symbolLengthPoints = int(symbolLengthSec * sampleRate)
    shift1 = data[0:-symbolLengthPoints]
    shift2 = np.conj(data[symbolLengthPoints:])
    x = np.angle(shift1 * shift2)
    times = np.arange(len(x)) / sampleRate
    plt.plot(times, x)
    plt.show()


def mix(sampleRate: float, data: np.ndarray):
    freq = 1000
    times = np.arange(len(data)) / sampleRate
    sin = np.sin(times * 2 * np.pi * freq)
    #plt.plot(times, sin * data + 40000)
    # plt.show()
    decode(sampleRate, data * sin)


def fftidea(sampleRate: float, data: np.ndarray):
    fft = np.fft.fft(data)
    index = 32  # 32 or 1379?
    print(fft[index])
    plt.plot(fft[index].real, fft[index].imag, '.', color='b', alpha=.5)


def plotiq(sampleRate: float, data: np.ndarray):
    plt.figure()
    samplesPerFrame = int(sampleRate / 31.25)
    frameOffsetSamples = samplesPerFrame + .205

    #window = np.hanning(samplesPerFrame)

    for i in range(500):
        first = 5668 + int(i * frameOffsetSamples)
        frameData = data[first:first+samplesPerFrame]
        #frameData *= window
        fftidea(sampleRate, frameData)

    # plt.figure()
    # plt.plot(frameData)


if __name__ == "__main__":
    sampleRate, data = wavfile.read('../data/sample-qpsk31-44100.wav')
    data = data / 65535

    trimSec = 20
    data = data[:trimSec * sampleRate]
    # plt.plot(data)

    plotiq(sampleRate, data)

    plt.show()
