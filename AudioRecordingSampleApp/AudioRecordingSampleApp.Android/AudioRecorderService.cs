using System;
using System.IO;
using Android.Media;
using AudioRecordingSampleApp.Droid;
using Xamarin.Forms;
using System.Threading;
using System.Collections.ObjectModel;

[assembly: Dependency(typeof(AudioService_Android))]
namespace AudioRecordingSampleApp.Droid
{

    public class AudioService_Android : IAudioService
    {
        MediaRecorder recorder;
        Timer timer;
        bool isRecording = false;
        bool isListening = false;
        string currentRecordingPath;

        public void StartRecordingOnVoice(Action<string> recordingFinishedCallback)
        {
            recorder = new MediaRecorder();
            recorder.SetAudioSource(AudioSource.Mic);
            recorder.SetOutputFormat(OutputFormat.ThreeGpp);
            recorder.SetAudioEncoder(AudioEncoder.AmrNb);

            var documentsFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var fileName = $"recording_{DateTime.Now.ToString("yyyyMMddHHmmss")}.3gp";
            var filePath = Path.Combine(documentsFolder, fileName);

            recorder.SetOutputFile(filePath);

            try
            {
                recorder.Prepare();
                recorder.Start();
                currentRecordingPath = filePath;
                isRecording = true;

                timer = new Timer(delegate
                {
                    if (!isListening && isRecording && recorder != null)
                    {
                        var amplitude = recorder.MaxAmplitude;
                        var decibels = 20 * Math.Log10(amplitude / 32767.0);

                        // Check if voice detected
                        if (decibels > -30) // Adjust threshold as needed
                        {
                            isListening = true;
                        }
                    }
                    else if (isListening && isRecording && recorder != null)
                    {
                        var amplitude = recorder.MaxAmplitude;
                        var decibels = 20 * Math.Log10(amplitude / 32767.0);

                        // Check if silence
                        if (decibels < -30) // Adjust threshold as needed
                        {
                            // Stop recording after 20 seconds of silence
                            StopRecording();
                            recordingFinishedCallback?.Invoke(currentRecordingPath);
                        }
                    }
                }, null, 0, 1000);
            }
            catch (Exception ex)
            {
                // Handle error
            }
        }

        public void StopRecording()
        {
            if (recorder != null)
            {
                recorder.Stop();
                recorder.Release();
                recorder.Dispose();
                recorder = null;
                isRecording = false;
                isListening = false;
            }

            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        public ObservableCollection<Recording> GetRecordings()
        {
            var recordings = new ObservableCollection<Recording>();
            var documentsFolder = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var files = Directory.GetFiles(documentsFolder, "recording*.3gp");
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
                var mediaPlayer = new MediaPlayer();
                mediaPlayer.SetDataSource(filePath);
                mediaPlayer.Prepare();
                mediaPlayer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing recording: " + ex.Message);
            }
        }
    }
}


