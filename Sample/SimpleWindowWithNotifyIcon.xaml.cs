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
    /// <summary>
    /// Interaction logic for SimpleWindowWithNotifyIcon.xaml
    /// </summary>
    public partial class SimpleWindowWithNotifyIcon : Window
    {
        //private Timer timerMonster;
        private Timer timerSingleClick;
        private Timer timerRefreshScreen;

        //private string pathSounds;
        //private string pathIcons;

        private readonly MonsterController MonsterController;

        public SimpleWindowWithNotifyIcon()
        {
            InitializeComponent();

            InitializePath();

            //InitializeConfiguration();

            //InitializeTimerMonster();

            InitializeTimerSingleClick();

            InitializeTimerRefreshScreen();

            //InitializeSoundPlayer();

            pouf = Properties.Resources.ResourceKeyTest;

            MonsterController = MonsterController.Instance;

            MonsterController.timeToDrink = ReadyToDrink;

            UpdateScreenFields();
        }

        string pouf;

        private void InitializePath()
        {
            //pathSounds = Path.Combine(Environment.CurrentDirectory, @"Sounds\");
            //pathIcons  = Path.Combine(Environment.CurrentDirectory, @"Icons\");
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

        private void InitializeTimerSingleClick()
        {
            timerSingleClick = new Timer
            {
                Interval = 1000,
                AutoReset = false
            };

            timerSingleClick.Elapsed += Elapsed_TimerSingleClick;
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
            if (WindowState != WindowState.Minimized)
            {
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;

                timerRefreshScreen.Stop();

                return;
            }

            UpdateScreenFields();
            
            timerRefreshScreen.Start();

            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            Topmost = true;

            Rect desktopWorkingArea = SystemParameters.WorkArea;
            int margin = 10;

            Show();

            Left = desktopWorkingArea.Right - (Width + margin);
            Top = desktopWorkingArea.Bottom - (Height + margin);

        }

        private void UpdateProgressBar()
        {
            if (MonsterController.MonsterInTheFridge)
            {
                int progressValue = MonsterController.CalculateProgressValue();

                Debug.WriteLine($"{DateTime.Now}: Debug - Progress: {progressValue}%");

                Dispatcher.Invoke(() =>
                {
                    progressBarReminder.Value = progressValue;
                });
            }
        }

        private string SelectAudioFile()
        {
            OpenFileDialog openFileDialog = new();

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return "";
        }

        private void UpdateScreenFields()
        {
            if (MonsterController.Configuration.ReminderDuration != null)
                monsterReminderDuration.Text = MonsterController.Configuration.ReminderDuration;

            if (MonsterController.Configuration.RegisterSound != null)
                textRegisterAudioFile.Text = MonsterController.Configuration.RegisterSound;

            if (MonsterController.Configuration.ReminderSound != null)
                textReminderAudioFile.Text = MonsterController.Configuration.ReminderSound;
        }

        private void UpdateConfiguration()
        {
            MonsterController.Configuration.ReminderDuration = monsterReminderDuration.Text;
            MonsterController.Configuration.RegisterSound = textRegisterAudioFile.Text;
            MonsterController.Configuration.ReminderSound = textReminderAudioFile.Text;
        }

        private void RegisterMonster()
        {
            MonsterController.RegisterMonster();
            MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\redmonsterlogo.ico");

            string formatedRefTime = string.Format("{0:T}", MonsterController.RefTime);
            textRegisteredAt.Text = $"{Properties.Resources.StatusBarTextRegisteredAt} {formatedRefTime}";
        }

        private void UnregisterMonster()
        {
            MonsterController.UnRegisteredMonster();
            MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            textRegisteredAt.Text = Properties.Resources.StatusBarTextNoReminder;
        }

        private void ReadyToDrink()
        {
            Dispatcher.Invoke(() =>
            {
                Debug.WriteLine($"Delegate TimeToDrink!!!");
                textRegisteredAt.Text = Properties.Resources.StatusBarTextReadyToDrink;
                MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            });
        }

        /* **********************************************************
         *  ELAPSED
         * ********************************************************** */
        private void Elapsed_TimerSingleClick(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Single Click Event!");

            /*
             * Will be executed from a thread different from the UI Element owner:
             * https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
             *
             */
            Dispatcher.Invoke(() =>
            {
                ToggleDisplay();
            });
        }

        private void Elapsed_TimerRefreshScreen(object sender, ElapsedEventArgs e)
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
            UnregisterMonster();
        }

        private void ImageClose_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            UpdateConfiguration();
            MonsterController.SaveConfiguration();
            Close();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            UnregisterMonster();
        }

        private void ButtonSelectReminderAudioFile_Click(object sender, RoutedEventArgs e)
        {
            textReminderAudioFile.Text = SelectAudioFile();
            MonsterController.Configuration.ReminderSound = textReminderAudioFile.Text;
        }

        private void ImageMinimize_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UpdateConfiguration();
            MonsterController.SaveConfiguration();
            ToggleDisplay();
        }

        private void ButtonSelectRegisterAudioFile_Click(object sender, RoutedEventArgs e)
        {

            textRegisterAudioFile.Text = SelectAudioFile();
            MonsterController.Configuration.RegisterSound = textRegisterAudioFile.Text;
        }

        private void ImagePlayRegisterSound_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //player.URL = textRegisterAudioFile.Text;
            MonsterController.player.URL = textRegisterAudioFile.Text;
        }

        private void ImagePlayReminderSound_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //player.URL = textReminderAudioFile.Text;
            MonsterController.player.URL = textReminderAudioFile.Text;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("---------------------------------");
            Debug.WriteLine($"pouf: {pouf}");

            ListSounds listSounds = new();
            listSounds.Show();

            int margin = 10;

            listSounds.Top = Top - (listSounds.Height - Height) - margin;
            listSounds.Left = Left - (listSounds.Width - Width) - margin;
        }

    }
}
