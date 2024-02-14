using System;
using System.Collections.ObjectModel;

namespace AudioRecordingSampleApp
{
    public interface IAudioService
    {
        void StartRecordingOnVoice(Action<string> recordingFinishedCallback);
        void StopRecording();
        ObservableCollection<Recording> GetRecordings();
        void PlayRecording(string filePath);
    }
}

