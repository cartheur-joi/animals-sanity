using System;
using System.IO;
using System.Threading.Tasks;
using Cartheur.Presents.Interfaces;
using Cartheur.Presents.Players;

namespace Cartheur.Presents
{
    internal class LinuxRecorder : UnixPlayerBase, IRecorder
    {
        protected override string GetBashCommand(string fileName)
        {
            if (Path.GetExtension(fileName).ToLower().Equals(".wav"))
            {
                return "arecord -q";
            }
            else if (Path.GetExtension(fileName).ToLower().Equals(".mp3"))
            {
                return "mpg123 -q";
            }
            else
            {
                return "arecord -q -fdat -d 5";
            }
        }
        protected override string BashCommandRecording(string fileName, int duration)
        {
            return "arecord -q -fdat -d " + duration.ToString();
        }
        /// <summary>
        /// Will start a recording session. The fileName parameter can be an absolute path or a path relative to the directory where the library is located, duration is the time recording should continue before saving the file. Sets Recording flag to true. Sets Paused flag to false.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override Task SetVolume(byte percent)
        {
            if (percent > 100)
                throw new ArgumentOutOfRangeException(nameof(percent), "Percent can't exceed 100");

            var tempProcess = StartBashProcess($"amixer -M set 'Master' {percent}%");
            tempProcess.WaitForExit();

            return Task.CompletedTask;
        }
    }
}
