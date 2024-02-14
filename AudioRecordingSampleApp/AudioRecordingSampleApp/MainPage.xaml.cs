using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AudioRecordingSampleApp
{
    public partial class MainPage : ContentPage
    {
        ObservableCollection<Recording> _recordings;
        ObservableCollection<Recording> Recordings
        {
            get { return _recordings; }
            set
            {
                _recordings = value;
                OnPropertyChanged();
            }
        }

        public ICommand PlayCommand { get; }

        public MainPage()
        {
            InitializeComponent();
            Recordings = new ObservableCollection<Recording>();
            BindingContext = this;

            PlayCommand = new Command<string>(filePath =>
            {
                // Handle playback of audio file with the provided file path
                // Example: DependencyService.Get<IAudioPlayer>().Play(filePath);
                DisplayAlert("Playback", $"Playing audio: {filePath}", "OK");
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadRecordings();
            StartListening();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopListening();
        }

        private void LoadRecordings()
        {
            Recordings.Clear();
            var recordings = new ObservableCollection<Recording>();
            var audioService = DependencyService.Get<IAudioService>();
            foreach (var recording in audioService.GetRecordings())
            {
                recordings.Add(recording);
            }
            Recordings = new ObservableCollection<Recording>(recordings);
        }

        private void StartListening()
        {
            var audioService = DependencyService.Get<IAudioService>();
            audioService.StartRecordingOnVoice((filePath) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LoadRecordings();
                });
            });
        }

        private void StopListening()
        {
            var audioService = DependencyService.Get<IAudioService>();
            audioService.StopRecording();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedRecording = e.Item as Recording;
            var audioService = DependencyService.Get<IAudioService>();
            audioService.PlayRecording(selectedRecording.FilePath);
        }
    }
}

