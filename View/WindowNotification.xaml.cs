using MonsterReminder.Controller;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using System.Windows;
using Timer = System.Timers.Timer;

namespace MonsterReminder.View
{
    /// <summary>
    /// Interaction logic for WindowNotification.xaml
    /// </summary>
    public partial class WindowNotification : Window
    {
        private Timer timerSingleClick;

        private readonly MonsterController MonsterController;
        private readonly WindowsController WindowsController;

        public WindowNotification()
        {
            InitializeComponent();

            InitializeTimerSingleClick();

            MonsterController = MonsterController.Instance;
            MonsterController.timeToDrink += ReadyToDrink;

            WindowsController = WindowsController.Instance;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //clean up notifyicon (would otherwise stay open until application finishes)
            MyNotifyIcon.Dispose();

            base.OnClosing(e);
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

        private void ReadyToDrink()
        {
            MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
        }

        /* **********************************************************
         *  ELAPSED
         * ********************************************************** */
        private void Elapsed_TimerSingleClick(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Single Click Event!");

            /*
             * As an Elapsed event from a timer, it will be
             * executed from a thread different from the UI Element owner:
             * https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
             *
             */
            Dispatcher.Invoke(() =>
            {
                //ToggleDisplay();
                WindowsController.ShowWindowInfo();
            });
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

        private void RegisterMonster()
        {
            MonsterController.RegisterMonster();
            MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\redmonsterlogo.ico");

            //string formatedRefTime = string.Format("{0:T}", MonsterController.RefTime);
            //textRegisteredAt.Text = $"{Properties.Resources.StatusBarTextRegisteredAt} {formatedRefTime}";
        }

        private void UnregisterMonster()
        {
            MonsterController.UnRegisteredMonster();
            MyNotifyIcon.Icon = new Icon(@"F:\Code\C#\MonsterReminder\Icons\MonsterLogo.ico");
            //textRegisteredAt.Text = Properties.Resources.StatusBarTextNoReminder;
        }

        private void DisplayListSounds()
        {
            ListSounds listSounds = new();
            listSounds.Show();

            //int margin = 10;

            //if (WindowState == WindowState.Minimized)
            //{
            //    Rect desktopWorkingArea = SystemParameters.WorkArea;

            //    listSounds.Left = desktopWorkingArea.Right - (listSounds.Width + margin);
            //    listSounds.Top = desktopWorkingArea.Bottom - (listSounds.Height + margin);
            //}
            //else
            //{
            //    // appear on the side:
            //    listSounds.Top = Top - (listSounds.Height - Height);
            //    listSounds.Left = Left - Width;
            //}
        }

        private void MenuItem_ConfigureSounds(object sender, RoutedEventArgs e)
        {
            //DisplayListSounds();
            WindowsController.ShowListSounds();
        }

        private void MenuItem_Quit(object sender, RoutedEventArgs e)
        {
            Quit();
        }

        private void Quit()
        {
            //UpdateConfiguration();
            MonsterController.SaveConfiguration();
            Close();
        }
    }
}
