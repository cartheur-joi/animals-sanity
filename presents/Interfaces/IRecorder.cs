using System;
using System.Threading.Tasks;

namespace Cartheur.Presents.Interfaces
{
    public interface IRecorder
    {
        event EventHandler RecordingFinished;

        bool Recording { get; }
        bool Paused { get; }

        Task Record(string fileName, int duration);
        Task Pause();
        Task Resume();
        Task Stop();
        Task SetVolume(byte percent);
    }
}
