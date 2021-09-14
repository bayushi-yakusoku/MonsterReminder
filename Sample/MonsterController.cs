using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
        private static readonly Lazy<MonsterController> lazy = new(() => new());
        public static MonsterController Instance => lazy.Value;
        private MonsterController()
        {
#if DEBUG
            Debug.WriteLine("private constructor!");
#endif

            player = new WMPLib.WindowsMediaPlayer();

            InitializeConfiguration();
            //InitializeSoundPlayer();
            InitializeTimerMonster();
            UploadConfiguration();
        }

        private string configurationFile;
        public Configuration Configuration { get; private set; }

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

            //if (configuration.ReminderDuration != null)
            //    monsterReminderDuration.Text = configuration.ReminderDuration;

            //if (configuration.RegisterSound != null)
            //    textRegisterAudioFile.Text = configuration.RegisterSound;

            //if (configuration.ReminderSound != null)
            //    textReminderAudioFile.Text = configuration.ReminderSound;
        }

        public void SaveConfiguration()
        {
            Configuration.LastUpdate = DateTime.Now;
            //configuration.ReminderDuration = monsterReminderDuration.Text;
            //configuration.RegisterSound = textRegisterAudioFile.Text;
            //configuration.ReminderSound = textReminderAudioFile.Text;

            JsonSerializerOptions options = new() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Configuration, options);

            File.WriteAllText(configurationFile, jsonString);
        }

        private Timer timerMonster;

        public readonly WMPLib.WindowsMediaPlayer player;

        private void InitializeSoundPlayer()
        {
            //audioFile = @"F:\Sons\Chansons\Various Artists\Chirac en prison.mp3";
            //audioFile = @"F:\Code\C#\MonsterReminder\Sounds\Abdos Par Vivi.3gp";

            //textRegisterAudioFile.Text = Path.Combine(pathSounds, "prepare_monster.3gp");
            //textReminderAudioFile.Text = Path.Combine(pathSounds, "chercher_monster.3gp");
        }

        private void InitializeTimerMonster()
        {
            timerMonster = new Timer
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

        public DateTime RefTime { get; private set; }
        int refDuration;
        public bool MonsterInTheFridge { get; private set; } = false;

        public void RegisterMonster()
        {
            RefTime = DateTime.Now;
            MonsterInTheFridge = true;
            //player.URL = textRegisterAudioFile.Text;
            player.URL = Configuration.RegisterSound;

            try
            {
                //refDuration = 60000 * Convert.ToInt32(monsterReminderDuration.Text);
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

            //MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\redmonsterlogo.ico");
            //MyNotifyIcon.Icon = new Icon("/Icons/redmonsterlogo.ico");
            //MyNotifyIcon.Icon = new Icon(Path.Combine(pathIcons, "redmonsterlogo.ico"));

            Debug.WriteLine($"{DateTime.Now}: Debug - Monster registered, reminder in {Configuration.ReminderDuration} minutes(s)");
        }

        private void MonsterIsReadyToDrink()
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Ring!");

            timeToDrink();

            timerMonster.Stop();

            MonsterInTheFridge = false;
            //player.URL = textReminderAudioFile.Text;
            player.URL = Configuration.ReminderSound;

            //MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            //MyNotifyIcon.Icon = new Icon(Path.Combine(pathIcons, "MonsterLogo.ico"));
        }

        public void UnRegisteredMonster()
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Remove Current Reminder!!");

            MonsterInTheFridge = false;
            timerMonster.Stop();

            //MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            //MyNotifyIcon.Icon = new Icon(Path.Combine(pathIcons, "MonsterLogo.ico"));
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

        //public delegate void TimeToDrink();
        //public TimeToDrink timeToDrink;
        public Action timeToDrink;
    }
}
