using Cartheur.Presents.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cartheur.Presents.Players
{
    internal abstract class UnixPlayerBase : IPlayer, IRecorder
    {
        private Process _process = null;
        internal const string PauseProcessCommand = "kill -STOP {0}";
        internal const string ResumeProcessCommand = "kill -CONT {0}";
        /// <summary>
        /// Occurs when the playback process has finished.
        /// </summary>
        public event EventHandler PlaybackFinished;
        /// <summary>
        /// Occurs when the recording process has finished.
        /// </summary>
        public event EventHandler RecordingFinished;
        /// <summary>
        /// Indicates whether the current process is playing.
        /// </summary>
        public bool Playing { get; private set; }
        /// <summary>
        /// Indicates whether the current process is recording.
        /// </summary>
        public bool Recording { get; private set; }
        /// <summary>
        /// Indicates whether the current process is paused.
        /// </summary>
        public bool Paused { get; private set; }

        protected abstract string GetBashCommand(string fileName);
        protected abstract string BashCommandRecording(string fileName, int duration);
        /// <summary>
        /// Records audio to the specified file for the specified duration.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public async Task Record(string filePath, int duration)
        {
            await Stop();
            var BashToolName = BashCommandRecording(filePath, duration);
            _process = StartBashProcess($"{BashToolName} '{filePath}'");
            _process.EnableRaisingEvents = true;
            _process.Exited += HandleRecordingFinished;
            _process.ErrorDataReceived += HandleRecordingFinished;
            _process.Disposed += HandleRecordingFinished;
            Recording = true;
        }
        /// <summary>
        /// Plays the specified file.
        /// </summary>
        /// <param name="fileName"></param>
        public async Task Play(string fileName)
        {
            await Stop();
            var BashToolName = GetBashCommand(fileName);
            _process = StartBashProcess($"{BashToolName} '{fileName}'");
            _process.EnableRaisingEvents = true;
            _process.Exited += HandlePlaybackFinished;
            _process.ErrorDataReceived += HandlePlaybackFinished;
            _process.Disposed += HandlePlaybackFinished;
            Playing = true;
        }
        /// <summary>
        /// Pauses the current process if it is playing.
        /// </summary>
        public Task Pause()
        {
            if (Playing && !Paused && _process != null)
            {
                var tempProcess = StartBashProcess(string.Format(PauseProcessCommand, _process.Id));
                tempProcess.WaitForExit();
                Paused = true;
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// Resumes the current process if it is paused.
        /// </summary>
        /// <returns></returns>
        public Task Resume()
        {
            if (Playing && Paused && _process != null)
            {
                var tempProcess = StartBashProcess(string.Format(ResumeProcessCommand, _process.Id));
                tempProcess.WaitForExit();
                Paused = false;
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// Stops the current process and disposes of it.
        /// </summary>
        /// <returns></returns>
        public Task Stop()
        {
            if (_process != null)
            {
                _process.Kill();
                _process.Dispose();
                _process = null;
            }

            Playing = false;
            Paused = false;

            return Task.CompletedTask;
        }
        protected Process StartBashProcess(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            return process;
        }
        internal void HandlePlaybackFinished(object sender, EventArgs e)
        {
            if (Playing)
            {
                Playing = false;
                PlaybackFinished?.Invoke(this, e);
            }
        }
        internal void HandleRecordingFinished(object sender, EventArgs e)
        {
            if (Recording)
            {
                Recording = false;
                RecordingFinished?.Invoke(this, e);
            }
        }
        /// <summary>
        /// Sets the volume of the player to the specified percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public abstract Task SetVolume(byte percent);
    }
}
