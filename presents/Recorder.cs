using Cartheur.Presents.Interfaces;
using System;
using System.Threading.Tasks;

namespace Cartheur.Presents
{
    /// <summary>
    /// The main class to handle recording audio files.
    /// </summary>
    /// <seealso cref="Cartheur.Presents.Interfaces.IRecorder" />
    public class Recorder : IRecorder
    {
        private readonly IRecorder _internalRecorder;
        /// <summary>
        /// Internally, sets Recording flag to false. Additional handlers can be attached to it to handle any custom logic.
        /// </summary>
        public event EventHandler RecordingFinished;
        /// <summary>
        /// Indicates that the audio is currently recording.
        /// </summary>
        public bool Recording => _internalRecorder.Recording;
        /// <summary>
        /// Indicates that the audio recording is currently paused.
        /// </summary>
        public bool Paused => _internalRecorder.Paused;

        /// <summary>
        /// Initializes a new instance of the <see cref="Recorder"/> class.
        /// </summary>
        /// <exception cref="Exception">No implementation exist for the current OS</exception>
        public Recorder()
        {
            _internalRecorder = new LinuxRecorder();
            _internalRecorder.RecordingFinished += OnRecordingFinished;
        }
        /// <summary>
        /// Will start a recording session. The fileName parameter can be an absolute path or a path relative to the directory where the library is located, duration is the time recording should continue before saving the file. Sets Recording flag to true. Sets Paused flag to false.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public async Task Record(string fileName, int duration)
        {
            await _internalRecorder.Record(fileName, duration);
        }
        /// <summary>
        /// Pauses any ongong recording. Sets Paused flag to true. Doesn't modify Recording flag.
        /// </summary>
        /// <returns></returns>
        public async Task Pause()
        {
            await _internalRecorder.Pause();
        }
        /// <summary>
        /// Resumes any paused recording. Sets Paused flag to false. Doesn't modify Recording flag.
        /// </summary>
        /// <returns></returns>
        public async Task Resume()
        {
            await _internalRecorder.Resume();
        }
        /// <summary>
        /// Stops any current recording and clears the buffer. Sets Recording and Paused flags to false.
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
            await _internalRecorder.Stop();
        }
        private void OnRecordingFinished(object sender, EventArgs e)
        {
            RecordingFinished?.Invoke(this, e);
        }
        /// <summary>
        /// Sets the playing volume as percent.
        /// </summary>
        public async Task SetVolume(byte percent)
        {
            await _internalRecorder.SetVolume(percent);
        }
    }
}
