using System;
using System.IO;
using Android.Media;
using AudioRecordingSampleApp.Droid;
using Xamarin;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioRecorderService))]
namespace AudioRecordingSampleApp.Droid
{

    public class AudioRecorderService : IAudioRecorderService
    {
        MediaRecorder recorder;
        string audioFilePath;

        public string StartRecording()
        {
            if (recorder != null)
            {
                recorder.Reset();
                recorder.Release();
                recorder.Dispose();
            }

            audioFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "audio.wav");

            recorder = new MediaRecorder();
            recorder.SetAudioSource(AudioSource.Mic);
            recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            recorder.SetAudioEncoder(AudioEncoder.AmrNb);
            recorder.SetOutputFile(audioFilePath);
            recorder.Prepare();
            recorder.Start();

            return audioFilePath;
        }

        public void StopRecording()
        {
            if (recorder != null)
            {
                recorder.Stop();
                recorder.Reset();
                recorder.Release();
                recorder.Dispose();
                recorder = null;
            }
        }

        public void Play(string filePath)
        {
            var mediaPlayer = new MediaPlayer();
            mediaPlayer.SetDataSource(filePath);
            mediaPlayer.Prepare();
            mediaPlayer.Start();
        }
    }

}

