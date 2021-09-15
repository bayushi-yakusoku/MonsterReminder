using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Timers;


namespace MonsterReminder.Sample
{
    class Configuration
    {
        public DateTimeOffset LastUpdate { get; set; }

        public string ReminderDuration { get; set; }
        public string ReminderSound { get; set; }
        public string RegisterSound { get; set; }
    }

    class MonsterController
    {
        /*
         * Lazy Singleton
         */
        private static readonly Lazy<MonsterController> lazy = new(() => new());
        public static MonsterController Instance => lazy.Value;
        private MonsterController()
        {
#if DEBUG
            Debug.WriteLine("Private Constructor!");
            Console.WriteLine("xxxxx");
#endif

            player = new WMPLib.WindowsMediaPlayer();

            InitializeConfiguration();
            InitializeTimerMonster();
            UploadConfiguration();
        }

        public Configuration Configuration { get; private set; }
        public DateTime RefTime { get; private set; }
        public bool MonsterInTheFridge { get; private set; }
        public readonly WMPLib.WindowsMediaPlayer player;
        public Action timeToDrink;

        private Timer timerMonster;
        private string configurationFile;
        private int refDuration;

        private void InitializeConfiguration()
        {
            string fileName = "configuration.json";

            configurationFile = Path.Combine(AppContext.BaseDirectory, "..", fileName);

            if (File.Exists(configurationFile))
            {
                Debug.WriteLine($"Configuration File exists!: {configurationFile}");
                UploadConfiguration();
            }
        }

        void UploadConfiguration()
        {
            string jsonString = File.ReadAllText(configurationFile);

            Configuration = JsonSerializer.Deserialize<Configuration>(jsonString);
        }

        public void SaveConfiguration()
        {
            Configuration.LastUpdate = DateTime.Now;

            JsonSerializerOptions options = new() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Configuration, options);

            File.WriteAllText(configurationFile, jsonString);
        }

        private void InitializeTimerMonster()
        {
            timerMonster = new()
            {
                Interval = 60000,
                AutoReset = false
            };

            timerMonster.Elapsed += Elapsed_TimerMonster;
        }

        private void Elapsed_TimerMonster(object sender, ElapsedEventArgs e)
        {
            MonsterIsReadyToDrink();
        }

        public void RegisterMonster()
        {
            RefTime = DateTime.Now;
            MonsterInTheFridge = true;
            player.URL = Configuration.RegisterSound;

            try
            {
                refDuration = 60000 * Convert.ToInt32(Configuration.ReminderDuration);
            }
            catch (Exception err)
            {
                Debug.WriteLine($"{DateTime.Now}: {err.Message}");

                throw;
            }

            timerMonster.Stop();
            timerMonster.Interval = refDuration;
            timerMonster.Start();

            Debug.WriteLine($"{DateTime.Now}: Debug - Monster registered, reminder in {Configuration.ReminderDuration} minutes(s)");
        }

        private void MonsterIsReadyToDrink()
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Ring!");

            timeToDrink();

            timerMonster.Stop();

            MonsterInTheFridge = false;
            player.URL = Configuration.ReminderSound;
        }

        public void UnRegisteredMonster()
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Remove Current Reminder!!");

            MonsterInTheFridge = false;
            timerMonster.Stop();
        }

        public int CalculateProgressValue()
        {
            DateTime currentTime = DateTime.Now;

            double msPassed = (currentTime - RefTime).TotalMilliseconds;

            double percent = (msPassed / refDuration) * 100;

            if (percent > 100)
                percent = 100;

            return (int)percent;
        }
    }
}
