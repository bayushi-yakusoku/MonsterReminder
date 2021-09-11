using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Timers;
using System.Windows;
using Timer = System.Timers.Timer;

namespace MonsterReminder.Sample
{
    public class Configuration
    {
        public DateTimeOffset LastUpdate { get; set; }

        public string ReminderDuration { get; set; }
        public string ReminderSound { get; set; }
        public string RegisterSound { get; set; }
    }
    
    
    /// <summary>
    /// Interaction logic for SimpleWindowWithNotifyIcon.xaml
    /// </summary>
    public partial class SimpleWindowWithNotifyIcon : Window
    {
        private Timer timerMonster;
        private Timer timerSingleClick;
        private Timer timerProgressBar;

        private string pathSounds;
        private string pathIcons;

        private string configurationFile;

        private SingleOne SingleOne;

        public SimpleWindowWithNotifyIcon()
        {
            InitializeComponent();

            InitializePath();

            InitializeConfiguration();

            InitializeTimerMonster();

            InitializeTimerSingleClick();

            InitializeTimerProgressBar();

            InitializeSoundPlayer();

            pouf = Properties.Resources.ResourceKeyTest;

            SingleOne = SingleOne.Instance;

            SingleOne.Name = "toto";
        }

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

        Configuration configuration = new();

        void UploadConfiguration()
        {
            string jsonString = File.ReadAllText(configurationFile);

            configuration = JsonSerializer.Deserialize<Configuration>(jsonString);

            if (configuration.ReminderDuration != null)
                monsterReminderDuration.Text = configuration.ReminderDuration;

            if (configuration.RegisterSound != null)
                textRegisterAudioFile.Text = configuration.RegisterSound;

            if (configuration.ReminderSound != null)
                textReminderAudioFile.Text = configuration.ReminderSound;
        }

        void SaveConfiguration()
        {
            configuration.LastUpdate = DateTime.Now;
            configuration.ReminderDuration = monsterReminderDuration.Text;
            configuration.RegisterSound = textRegisterAudioFile.Text;
            configuration.ReminderSound = textReminderAudioFile.Text;

            JsonSerializerOptions options = new() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(configuration, options);

            File.WriteAllText(configurationFile, jsonString);
        }

        string pouf;

        private void InitializePath()
        {
            pathSounds = Path.Combine(Environment.CurrentDirectory, @"Sounds\");
            pathIcons  = Path.Combine(Environment.CurrentDirectory, @"Icons\");
        }

        private void InitializeTimerProgressBar()
        {
            timerProgressBar = new Timer
            {
                Interval = 1000,
                AutoReset = true
            };

            timerProgressBar.Elapsed += Elapsed_TimerProgressBar;
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

        private void InitializeTimerSingleClick()
        {
            timerSingleClick = new Timer
            {
                Interval = 1000,
                AutoReset = false
            };

            timerSingleClick.Elapsed += Elapsed_TimerSingleClick;
        }

        WMPLib.WindowsMediaPlayer player;

        //string reminderAudioFile;

        private void InitializeSoundPlayer()
        {
            player = new WMPLib.WindowsMediaPlayer();

            //audioFile = @"F:\Sons\Chansons\Various Artists\Chirac en prison.mp3";
            //audioFile = @"F:\Code\C#\MonsterReminder\Sounds\Abdos Par Vivi.3gp";

            //textRegisterAudioFile.Text = Path.Combine(pathSounds, "prepare_monster.3gp");
            //textReminderAudioFile.Text = Path.Combine(pathSounds, "chercher_monster.3gp");
        }

        /*
         * Due to known bug:
         * https://stackoverflow.com/questions/37333952/wpf-sizetocontent-widthandheight-windowstate-minimized-bug
         * 
         */
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            InvalidateMeasure();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //clean up notifyicon (would otherwise stay open until application finishes)
            MyNotifyIcon.Dispose();

            base.OnClosing(e);
        }

        private void ToggleDisplay()
        {
            /*
             * Will be executed from a thread different from the UI Element owner:
             * https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
             *
             */
            Dispatcher.Invoke(() =>
            {
                if (WindowState != WindowState.Minimized)
                {
                    WindowState = WindowState.Minimized;
                    ShowInTaskbar = false;

                    timerProgressBar.Stop();

                    return;
                }

                if (monsterInTheFridge)
                {
                    UpdateProgressBar();
                    timerProgressBar.Start();
                }

                WindowState = WindowState.Normal;
                ShowInTaskbar = true;
                Topmost = true;

                Rect desktopWorkingArea = SystemParameters.WorkArea;
                int margin = 10;

                Show();

                Left = desktopWorkingArea.Right - (Width + margin);
                Top = desktopWorkingArea.Bottom - (Height + margin);
            });
        }

        DateTime refTime;
        int refDuration;
        bool monsterInTheFridge = false;

        private void RegisterMonster()
        {
            refTime = DateTime.Now;
            monsterInTheFridge = true;
            player.URL = textRegisterAudioFile.Text;
            progressBarReminder.Value = 0;

            if (WindowState != WindowState.Minimized)
                timerProgressBar.Start();

            string formatedRefTime = string.Format("{0:T}", refTime);

            textRegisteredAt.Text = $"Registered at {formatedRefTime}";

            try
            {
                refDuration = 60000 * Convert.ToInt32(monsterReminderDuration.Text);
            }
            catch (Exception err)
            {
                Debug.WriteLine($"{DateTime.Now}: {err.Message}");

                throw;
            }

            timerMonster.Stop();
            timerMonster.Interval = refDuration;
            timerMonster.Start();

            MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\redmonsterlogo.ico");
            //MyNotifyIcon.Icon = new Icon("/Icons/redmonsterlogo.ico");
            //MyNotifyIcon.Icon = new Icon(Path.Combine(pathIcons, "redmonsterlogo.ico"));

            Debug.WriteLine($"{DateTime.Now}: Debug - Monster registered, reminder in {monsterReminderDuration.Text} second(s)");
        }

        private void MonsterIsReadyToDrink()
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Ring!");

            timerMonster.Stop();
            timerProgressBar.Stop();
            monsterInTheFridge = false;

            Dispatcher.Invoke(() =>
            {
                progressBarReminder.Value = 100;
                textRegisteredAt.Text = $"Ready To Drink!";
                player.URL = textReminderAudioFile.Text;
            });

            MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            //MyNotifyIcon.Icon = new Icon(Path.Combine(pathIcons, "MonsterLogo.ico"));
        }

        private void UnRegisteredMonster()
        {
            monsterInTheFridge = false;
            timerMonster.Stop();
            timerProgressBar.Stop();

            Dispatcher.Invoke(() =>
            {
                progressBarReminder.Value = 0;
                textRegisteredAt.Text = $"Nothing To Drink!";
            });

            MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            //MyNotifyIcon.Icon = new Icon(Path.Combine(pathIcons, "MonsterLogo.ico"));
        }

        private int CalculateProgressValue()
        {
            DateTime currentTime = DateTime.Now;

            double msPassed = (currentTime - refTime).TotalMilliseconds;

            double percent = (msPassed / refDuration) * 100;

            if (percent > 100)
                percent = 100;

            return (int)percent;
        }

        private void UpdateProgressBar()
        {
            int progressValue = CalculateProgressValue();

            Debug.WriteLine($"{DateTime.Now}: Debug - Progress: {progressValue}%");

            Dispatcher.Invoke(() =>
            {
                progressBarReminder.Value = progressValue;
            });
        }

        private string SelectAudioFile()
        {
            OpenFileDialog openFileDialog = new();

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return "";
        }

        /* **********************************************************
         *  ELAPSED
         * ********************************************************** */
        private void Elapsed_TimerSingleClick(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Single Click Event!");

            ToggleDisplay();
        }

        private void Elapsed_TimerMonster(object sender, ElapsedEventArgs e)
        {
            MonsterIsReadyToDrink();
        }

        private void Elapsed_TimerProgressBar(object sender, ElapsedEventArgs e)
        {
            UpdateProgressBar();
        }

        /* **********************************************************
         *  EVENTS
         * ********************************************************** */

        private void MyNotifyIcon_LeftClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            timerSingleClick.Stop();
            timerSingleClick.Start();
        }

        private void MyNotifyIcon_RightClick(object sender, RoutedEventArgs e)
        {

        }

        private void MyNotifyIcon_DoubleClick(object sender, RoutedEventArgs e)
        {
            // preventing single click event to be fired after this double click
            //e.Handled = true;

            // Cancel Single click timer:
            timerSingleClick.Stop();
            Debug.WriteLine($"{DateTime.Now}: Debug - Double Click Event");

            RegisterMonster();
        }

        private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Remove!!");

            UnRegisteredMonster();
        }

        private void ImageClose_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            SaveConfiguration();
            Close();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            player.controls.stop();

            UnRegisteredMonster();
        }

        private void ButtonSelectReminderAudioFile_Click(object sender, RoutedEventArgs e)
        {
            textReminderAudioFile.Text = SelectAudioFile();
        }

        private void ImageMinimize_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SaveConfiguration();
            ToggleDisplay();
        }

        private void ButtonSelectRegisterAudioFile_Click(object sender, RoutedEventArgs e)
        {
            textRegisterAudioFile.Text = SelectAudioFile();
        }

        private void ImagePlayRegisterSound_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            player.URL = textRegisterAudioFile.Text;
        }

        private void ImagePlayReminderSound_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            player.URL = textReminderAudioFile.Text;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("---------------------------------");
            Debug.WriteLine($"pouf: {pouf}");

            ListSounds listSounds = new ();
            listSounds.Show();

            int margin = 10;

            listSounds.Top = Top - (listSounds.Height - Height) - margin;
            listSounds.Left = Left - (listSounds.Width - Width) - margin;
        }

    }
}
