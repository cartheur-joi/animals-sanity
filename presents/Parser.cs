using System;
using System.IO;

namespace EmotionClassification
{
    public class Parser
    {
        public (string Modality, string VocalChannel, string Emotion, string EmotionalIntensity, string Statement, string Repetition, string Actor) ParseFileName(string fileName)
        {
            var parts = fileName.Split('-');
            if (parts.Length != 7)
            {
                throw new ArgumentException("Invalid filename format.");
            }

            return (
                Modality: parts[0],
                VocalChannel: parts[1],
                Emotion: parts[2],
                EmotionalIntensity: parts[3],
                Statement: parts[4],
                Repetition: parts[5],
                Actor: parts[6]
            );
        }

        public byte[] LoadAudioData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Audio file not found.", filePath);
            }

            return File.ReadAllBytes(filePath);
        }
    }
}