using System;
using System.IO;
using AudioRecordingSampleApp.iOS;
using AVFoundation;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioRecorderService))]
namespace AudioRecordingSampleApp.iOS
{

public class AudioRecorderService : IAudioRecorderService
    {
        AVAudioRecorder recorder;
        NSDictionary recordingSettings;
        NSUrl audioFilePath;

        public string StartRecording()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var audioFileName = $"audio_{DateTime.Now.ToString("yyyyMMddHHmmss")}.wav";
            audioFilePath = NSUrl.FromFilename(Path.Combine(documents, audioFileName));

            recordingSettings = new NSDictionary(AVAudioSettings.AVFormatIDKey, NSNumber.FromInt32((int)AudioToolbox.AudioFormatType.LinearPCM),
                                                    AVAudioSettings.AVSampleRateKey, NSNumber.FromFloat(44100.0f),
                                                    AVAudioSettings.AVNumberOfChannelsKey, NSNumber.FromInt32(2),
                                                    AVAudioSettings.AVLinearPCMBitDepthKey, NSNumber.FromInt32(16),
                                                    AVAudioSettings.AVLinearPCMIsBigEndianKey, NSNumber.FromBoolean(false),
                                                    AVAudioSettings.AVLinearPCMIsFloatKey, NSNumber.FromBoolean(false));

            recorder = AVAudioRecorder.Create(audioFilePath, new AudioSettings(recordingSettings), out NSError error);
            if (error != null)
            {
                Console.WriteLine("Error initializing recording: " + error.LocalizedDescription);
                return null;
            }

            recorder.Record();
            return audioFilePath.Path;
        }

        public void StopRecording()
        {
            recorder?.Stop();
            recorder?.Dispose();
            recorder = null;
        }

        public void Play(string filePath)
        {
            var player = AVAudioPlayer.FromUrl(NSUrl.FromFilename(filePath));
            player.PrepareToPlay();
            player.Play();
        }
    }


}
