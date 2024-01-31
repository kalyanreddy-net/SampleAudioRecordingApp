using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AudioRecordingSampleApp
{
    public partial class MainPage : ContentPage
    {
        private bool isRecording = false;
        private string audioFilePath;
        private List<Item> audioRecordings = App.Database.GetItems();

        public List<Item> AudioRecordings
        {
            get { return audioRecordings; }
            set
            {
                audioRecordings = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            audioRecordings = App.Database.GetItems();
            recordingsList.ItemsSource = new List<Item>(audioRecordings);
        }

        async void OnRecordAudioButtonClicked(object sender, EventArgs e)
        {
            if (!isRecording)
            {
                isRecording = true;
                recordButton.Text = "Pause";

                var audioFilePath = DependencyService.Get<IAudioRecorderService>().StartRecording();

                // Prompt the user to enter a name for the recorded audio clip
                string itemName = await DisplayPromptAsync("New Item", "Enter a name for the audio clip", "OK", "Cancel");

                if (itemName != null) // User clicked "OK"
                {
                    // Create a new Item object with the audio file path and name
                    var newItem = new Item
                    {
                        Name = itemName,
                        AudioFilePath = audioFilePath
                    };

                    App.Database.SaveItem(newItem);
                }
                audioRecordings = App.Database.GetItems();
                recordingsList.ItemsSource = new List<Item>(audioRecordings);
            }
            else
            {
                isRecording = false;
                recordButton.Text = "Record Audio";
                DependencyService.Get<IAudioRecorderService>().StopRecording();
                audioRecordings = App.Database.GetItems();
                recordingsList.ItemsSource = new List<Item>(audioRecordings);
            }
        }

        private void OnPlayButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var item = button?.BindingContext as Item;

            if (item != null && !string.IsNullOrEmpty(item.AudioFilePath))
            {
                // Use platform-specific APIs to play audio
                DependencyService.Get<IAudioRecorderService>().Play(item.AudioFilePath);
            }
        }
    }
}

