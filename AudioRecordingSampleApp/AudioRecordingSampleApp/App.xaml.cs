using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AudioRecordingSampleApp
{
    public partial class App : Application
    {

        static ItemDatabase database;

        public static ItemDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new ItemDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ItemsSQLite.db3"));
                }
                return database;
            }
        }


        public App ()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

