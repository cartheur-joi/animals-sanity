//
// This autonomous intelligent system software is the property of Cartheur Research B.V. Copyright 2024 - 2026, all rights reserved.
//
using System.IO;

namespace Boagaphish.Analytics
{
    static class Helpers
    {
        // Convert two-bytes to one double in the range -1 to 1.
        static double BytesToDouble(byte firstByte, byte secondByte)
        {
            // Convert two bytes to one short (little endian).
            short s = (short)((secondByte << 8) | firstByte);

            // Convert to range from -1 to (just below) 1.
            return s / 32768.0;
        }     
        /// <summary>
        /// Returns left and right double arrays. 'right' will be null if sound is mono. 
        /// </summary>
        /// <param name="filename">The audio filename as path.</param>
        /// <param name="left">The left array.</param>
        /// <param name="right">The right array.</param>
        public static void ReturnWaveData(string filename, out double[] left, out double[] right)
        {
            byte[] wave = File.ReadAllBytes(filename);
            // Determine if mono or stereo
            int channels = wave[22];     // Forget byte 23 as 99.999% of WAVs are 1 or 2 channels.
            // Get past all the other sub chunks to get to the data subchunk.
            int position = 12;   // First Subchunk ID from 12 to 16.
            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal)).
            while (!(wave[position] == 100 && wave[position + 1] == 97 && wave[position + 2] == 116 && wave[position + 3] == 97))
            {
                position += 4;
                int chunkSize = wave[position] + wave[position + 1] * 256 + wave[position + 2] * 65536 + wave[position + 3] * 16777216;
                position += 4 + chunkSize;
            }
            position += 8;
            // Pos is now positioned to start of actual sound data.
            int samples = (wave.Length - position) / 2;     // 2 bytes per sample (16-bit sound mono).

            if (channels == 2)
            {
                samples /= 2;        // 4 bytes per sample (16-bit stereo)
            }
            // Allocate memory (right will be null if only mono sound).
            left = new double[samples];

            if (channels == 2)
            {
                right = new double[samples];
            }
            else
            {
                right = null;
            }
            // Write to two double arrays.
            int i = 0;
            while (position < wave.Length)
            {
                left[i] = BytesToDouble(wave[position], wave[position + 1]);
                position += 2;

                if (channels == 2)
                {
                    right[i] = BytesToDouble(wave[position], wave[position + 1]);
                    position += 2;
                }
                i++;
            }
        }
    }
}
