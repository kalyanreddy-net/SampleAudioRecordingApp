using System;
namespace AudioRecordingSampleApp
{
    public interface IAudioRecorderService
    {
        string StartRecording();
        void StopRecording();
        void Play(string filePath);
    }
}

