using MonsterReminder.Controller;
using MonsterReminder.Tools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using Timer = System.Timers.Timer;

namespace MonsterReminder.View
{
    /// <summary>
    /// Interaction logic for SimpleWindowWithNotifyIcon.xaml
    /// </summary>
    public partial class WindowInfo : Window
    {
        private Timer timerRefreshScreen;

        private readonly MonsterController monsterController;
        private readonly WindowsController windowsController;

        public WindowInfo()
        {
            InitializeTimerRefreshScreen();

            monsterController = MonsterController.Instance;
            windowsController = WindowsController.Instance;

            monsterController.timeToDrink += ReadyToDrink;

            InitializeComponent();

        }

        private void InitializeTimerRefreshScreen()
        {
            timerRefreshScreen = new Timer
            {
                Interval = 1000,
                AutoReset = true
            };

            timerRefreshScreen.Elapsed += Elapsed_TimerRefreshScreen;
        }

        private void UpdateStatusBar()
        {
            if (monsterController.MonsterInTheFridge)
            {
                int progressValue = monsterController.CalculateProgressValue();
                string formatedRefTime = string.Format("{0:T}", monsterController.RefTime);

                Log.Debug($"Progress: {progressValue}%");

                Dispatcher.Invoke(() =>
                {
                    progressBarReminder.Value = progressValue;
                    textRegisteredAt.Text = $"{Properties.Resources.StatusBarTextRegisteredAt} {formatedRefTime}";

                });
            }
        }

        private void UpdateScreenFields()
        {
            if (monsterController.Configuration.ReminderDuration != null)
            {
                monsterReminderDuration.Text = monsterController.Configuration.ReminderDuration;
            }

            if (monsterController.Configuration.RegisterSound != null)
            {
                textRegisterAudioFile.Text = monsterController.Configuration.RegisterSound;
            }

            if (monsterController.Configuration.ReminderSound != null)
            {
                textReminderAudioFile.Text = monsterController.Configuration.ReminderSound;
            }
        }

        private void UpdateConfiguration()
        {
            monsterController.Configuration.ReminderDuration = monsterReminderDuration.Text;
            monsterController.Configuration.RegisterSound = textRegisterAudioFile.Text;
            monsterController.Configuration.ReminderSound = textReminderAudioFile.Text;
        }

        private void UnregisterMonster()
        {
            monsterController.UnRegisteredMonster();
            textRegisteredAt.Text = Properties.Resources.StatusBarTextNoReminder;
        }

        private void ReadyToDrink()
        {
            Log.Debug("Delegate TimeToDrink!!!");

            Dispatcher.Invoke(() =>
            {
                textRegisteredAt.Text = Properties.Resources.StatusBarTextReadyToDrink;
            });
        }

        private void Quit()
        {
            UpdateConfiguration();
            monsterController.SaveConfiguration();
            Close();
        }

        /* **********************************************************
         *  ELAPSED
         * ********************************************************** */

        private void Elapsed_TimerRefreshScreen(object sender, ElapsedEventArgs e)
        {
            UpdateStatusBar();
        }

        /* **********************************************************
         *  EVENTS
         * ********************************************************** */

        /*
         * Due to known bug:
         * https://stackoverflow.com/questions/37333952/wpf-sizetocontent-widthandheight-windowstate-minimized-bug
         */
        protected override void OnStateChanged(EventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - OnStateChanged");

            base.OnStateChanged(e);

            InvalidateMeasure();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Log.Debug("Event");

            timerRefreshScreen.Dispose();

            base.OnClosing(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            Log.Debug("Event");

            base.OnInitialized(e);

            UpdateScreenFields();
            UpdateStatusBar();

            timerRefreshScreen.Start();
        }

        private void ImageClose_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            Quit();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            UnregisterMonster();
        }

        private void ButtonSelectReminderAudioFile_Click(object sender, RoutedEventArgs e)
        {
            textReminderAudioFile.Text = Tool.SelectFile();

            monsterController.Configuration.ReminderSound = textReminderAudioFile.Text;
        }

        private void ButtonSelectRegisterAudioFile_Click(object sender, RoutedEventArgs e)
        {

            textRegisterAudioFile.Text = Tool.SelectFile();

            monsterController.Configuration.RegisterSound = textRegisterAudioFile.Text;
        }

        private void ImagePlayRegisterSound_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            monsterController.player.URL = textRegisterAudioFile.Text;
        }

        private void ImagePlayReminderSound_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            monsterController.player.URL = textReminderAudioFile.Text;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            windowsController.ShowListSounds();
        }
    }
}
