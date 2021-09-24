using MonsterReminder.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Timers;


namespace MonsterReminder.Controller
{
    class Configuration
    {
        public DateTimeOffset LastUpdate { get; set; }
        public string ReminderDuration { get; set; }
        public string ReminderSound { get; set; }
        public string RegisterSound { get; set; }
        public List<string> ReminderSounds { get; set; } = new();
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
            Log.Debug("Private Constructor!");
#endif

            player = new WMPLib.WindowsMediaPlayer();
            random = new();

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
        private readonly Random random;

        private void InitializeConfiguration()
        {
            string fileName = "configuration.json";

            configurationFile = Path.Combine(AppContext.BaseDirectory, "..", fileName);

            if (File.Exists(configurationFile))
            {
                Log.Debug($"Configuration File exists!: {configurationFile}");
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
                Log.Debug($"{err.Message}");

                throw;
            }

            timerMonster.Stop();
            timerMonster.Interval = refDuration;
            timerMonster.Start();

            Log.Debug($"Monster registered, reminder in {Configuration.ReminderDuration} minutes(s)");
        }

        private void MonsterIsReadyToDrink()
        {
            Log.Debug("Debug - Ring!");

            timeToDrink?.Invoke();

            timerMonster.Stop();

            MonsterInTheFridge = false;

            PlayOneRandomReminder();
        }

        public void UnRegisteredMonster()
        {
            Log.Debug("Remove Current Reminder!!");

            MonsterInTheFridge = false;
            timerMonster.Stop();
        }

        public int CalculateProgressValue()
        {
            DateTime currentTime = DateTime.Now;

            double msPassed = (currentTime - RefTime).TotalMilliseconds;

            double percent = (msPassed / refDuration) * 100;

            if (percent > 100)
            {
                percent = 100;
            }

            return (int)percent;
        }

        public bool AddReminderSound(string sound)
        {
            if (Configuration.ReminderSounds.Exists(s => s == sound))
            {
                return false;
            }

            Configuration.ReminderSounds.Add(sound);
            return true;
        }

        public bool RemoveReminderSound(string sound) => Configuration.ReminderSounds.Remove(sound);

        public void PlayOneRandomReminder()
        {
            int limit = Configuration.ReminderSounds.Count;

            int choosen = random.Next(limit);

            player.URL = Configuration.ReminderSounds[choosen];

            Debug.WriteLine($"Sound played: n°{choosen} - {Configuration.ReminderSounds[choosen]}");
        }
    }
}
