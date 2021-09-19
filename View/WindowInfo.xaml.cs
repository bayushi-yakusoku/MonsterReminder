using Microsoft.Win32;
using MonsterReminder.Controller;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
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
        //private Timer timerSingleClick;
        private Timer timerRefreshScreen;

        private readonly MonsterController MonsterController;

        public WindowInfo()
        {
            InitializeTimerRefreshScreen();

            MonsterController = MonsterController.Instance;

            MonsterController.timeToDrink += ReadyToDrink;

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

        /*
         * Due to known bug:
         * https://stackoverflow.com/questions/37333952/wpf-sizetocontent-widthandheight-windowstate-minimized-bug
         * 
         */
        protected override void OnStateChanged(EventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - OnStateChanged");

            base.OnStateChanged(e);

            InvalidateMeasure();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - OnClosing");
            Log("!!Debug - OnClosing");

            timerRefreshScreen.Dispose();

            base.OnClosing(e);
        }
        protected override void OnInitialized(EventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - OnInitialized");

            base.OnInitialized(e);

            UpdateScreenFields();
            UpdateStatusBar();

            timerRefreshScreen.Start();
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

        private void UpdateStatusBar()
        {
            if (MonsterController.MonsterInTheFridge)
            {
                int progressValue = MonsterController.CalculateProgressValue();
                string formatedRefTime = string.Format("{0:T}", MonsterController.RefTime);

                Debug.WriteLine($"{DateTime.Now}: Debug - Progress: {progressValue}%");

                Dispatcher.Invoke(() =>
                {
                    progressBarReminder.Value = progressValue;
                    textRegisteredAt.Text = $"{Properties.Resources.StatusBarTextRegisteredAt} {formatedRefTime}";

                });
            }
        }

        private static string SelectFile()
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

        private void UnregisterMonster()
        {
            MonsterController.UnRegisteredMonster();
            //MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            textRegisteredAt.Text = Properties.Resources.StatusBarTextNoReminder;
        }

        private void ReadyToDrink()
        {
            Dispatcher.Invoke(() =>
            {
                Debug.WriteLine($"Delegate TimeToDrink!!!");
                textRegisteredAt.Text = Properties.Resources.StatusBarTextReadyToDrink;
                //MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            });
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
            textReminderAudioFile.Text = SelectFile();
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

            textRegisterAudioFile.Text = SelectFile();
            MonsterController.Configuration.RegisterSound = textRegisterAudioFile.Text;
        }

        private void ImagePlayRegisterSound_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MonsterController.player.URL = textRegisterAudioFile.Text;
        }

        private void ImagePlayReminderSound_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MonsterController.player.URL = textReminderAudioFile.Text;
        }

        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            DisplayListSounds();
        }

        private void DisplayListSounds()
        {
            ListSounds listSounds = new();
            listSounds.Show();

            int margin = 10;

            if (WindowState == WindowState.Minimized)
            {
                Rect desktopWorkingArea = SystemParameters.WorkArea;

                listSounds.Left = desktopWorkingArea.Right - (listSounds.Width + margin);
                listSounds.Top = desktopWorkingArea.Bottom - (listSounds.Height + margin);
            }
            else
            {
                // appear on the side:
                listSounds.Top = Top - (listSounds.Height - Height);
                listSounds.Left = Left - Width;
            }
        }

        private void Quit()
        {
            UpdateConfiguration();
            MonsterController.SaveConfiguration();
            Close();
        }

        public static void Log(string message, 
            [CallerFilePath] string file = "", 
            [CallerLineNumber] int line = 0, 
            [CallerMemberName] string member = "")
        {
            var s = string.Format("{0}:{1} - {2}: {3}", file, line, member, message);

            Debug.WriteLine(s);
            Console.WriteLine(s);
        }
    }
}
