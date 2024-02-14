using System;
using System.Collections.ObjectModel;
using System.IO;
using AudioRecordingSampleApp.iOS;
using AudioToolbox;
using AVFoundation;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService_iOS))]
namespace AudioRecordingSampleApp.iOS
{

    public class AudioService_iOS : IAudioService
    {
        AVAudioRecorder audioRecorder;
        NSTimer timer;
        bool isRecording = false;
        bool isListening = false;
        string currentRecordingPath;

        public void StartRecordingOnVoice(Action<string> recordingFinishedCallback)
        {
            var audioSettings = new AudioSettings
            {
                SampleRate = 44100,
                Format = AudioFormatType.LinearPCM,
                NumberChannels = 1,
                AudioQuality = AVAudioQuality.High
            };

            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = $"recording_{DateTime.Now.ToString("yyyyMMddHHmmss")}.wav";
            var filePath = Path.Combine(documentsFolder, fileName);

            var url = NSUrl.FromFilename(filePath);
            var session = AVAudioSession.SharedInstance();

            session.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            session.SetActive(true);

            var recordSettings = new AudioSettings
            {
                Format = audioSettings.Format,
                SampleRate = audioSettings.SampleRate,
                NumberChannels = audioSettings.NumberChannels
            };

            NSError error;
            audioRecorder = AVAudioRecorder.Create(url, recordSettings, out error);

            if (audioRecorder != null)
            {
                currentRecordingPath = filePath;
                audioRecorder.Record();
                isRecording = true;

                timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromSeconds(1), delegate
                {
                    if (!isListening && isRecording && audioRecorder != null)
                    {
                        audioRecorder.UpdateMeters();
                        var decibels = audioRecorder.AveragePower(0);

                        // Check if voice detected
                        if (decibels > -30) // Adjust threshold as needed
                        {
                            isListening = true;
                        }
                    }
                    else if (isListening && isRecording && audioRecorder != null)
                    {
                        audioRecorder.UpdateMeters();
                        var decibels = audioRecorder.AveragePower(0);

                        // Check if silence
                        if (decibels < -30) // Adjust threshold as needed
                        {
                            // Stop recording after 20 seconds of silence
                            StopRecording();
                            recordingFinishedCallback?.Invoke(currentRecordingPath);
                        }
                    }
                });
            }
            else
            {
                // Handle error
            }
        }

        public void StopRecording()
        {
            if (audioRecorder != null)
            {
                audioRecorder.Stop();
                audioRecorder.Dispose();
                audioRecorder = null;
                isRecording = false;
                isListening = false;
            }

            if (timer != null)
            {
                timer.Invalidate();
                timer.Dispose();
                timer = null;
            }
        }

        public ObservableCollection<Recording> GetRecordings()
        {
            var recordings = new ObservableCollection<Recording>();
            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var files = Directory.GetFiles(documentsFolder, "recording*.wav");
            foreach (var file in files)
            {
                recordings.Add(new Recording { FilePath = file });
            }
            return recordings;
        }

        public void PlayRecording(string filePath)
        {
            try
            {
                var audioPlayer = AVAudioPlayer.FromUrl(NSUrl.FromFilename(filePath));
                audioPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing recording: " + ex.Message);
            }
        }
    }
}

