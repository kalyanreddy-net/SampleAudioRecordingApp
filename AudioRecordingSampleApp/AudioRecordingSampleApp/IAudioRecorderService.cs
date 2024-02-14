using System;
namespace AudioRecordingSampleApp
{
    public interface IAudioRecorderService
    {
        void StartRecordingOnVoice();
        void StopRecording();
    }
}

