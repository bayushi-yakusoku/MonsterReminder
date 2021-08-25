using System;
using System.ComponentModel;
using System.Diagnostics;
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
        private Timer timerMonster;
        private Timer timerSingleClick;

        public SimpleWindowWithNotifyIcon()
        {
            InitializeComponent();

            InitializeTimer();

            InitializeTimerSingleClick();

            InitializeSoundPlayer();
        }

        private void InitializeTimer()
        {
            timerMonster = new Timer
            {
                Interval = 60000,
                AutoReset = false
            };

            timerMonster.Elapsed += TimerMonster_Elapsed;
        }

        private void InitializeTimerSingleClick()
        {
            timerSingleClick = new Timer
            {
                Interval = 1000,
                AutoReset = false
            };

            timerSingleClick.Elapsed += TimerSingleClick_Elapsed;
        }

        WMPLib.WindowsMediaPlayer player;

        private void InitializeSoundPlayer()
        {
            player = new WMPLib.WindowsMediaPlayer();

            //player.URL = @"F:\Sons\Chansons\Various Artists\Chirac en prison.mp3";
            player.URL = @"F:\Code\C#\MonsterReminder\Sounds\Abdos Par Vivi.3gp";
            
            player.controls.stop();
        }

        private void TimerMonster_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Ring!");
            player.controls.play();
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
            int margin = 10;

            if (WindowState != WindowState.Minimized)
            {
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;

                return;
            }

            Rect desktopWorkingArea = SystemParameters.WorkArea;

            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
            Topmost = true;

            Left = desktopWorkingArea.Right - (Width + margin);
            Top = desktopWorkingArea.Bottom - (Height + margin);
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

        private void TimerSingleClick_Elapsed(object sender, ElapsedEventArgs e)
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

        private void MyNotifyIcon_RightClick(object sender, RoutedEventArgs e)
        {

        }

        private void MyNotifyIcon_DoubleClick(object sender, RoutedEventArgs e)
        {
            // preventing single click event to be fired after this double click
            //e.Handled = true;

            timerSingleClick.Stop();

            int duration;
            try
            {
                duration = Convert.ToInt32(monsterReminderDuration.Text);
            }
            catch (Exception err)
            {
                Debug.WriteLine($"{DateTime.Now}: {err.Message}");
                
                throw;
            }

            timerMonster.Stop();
            timerMonster.Interval = duration * 1000;
            timerMonster.Start();

            Debug.WriteLine($"{DateTime.Now}: Debug - Double Click Event");

            Debug.WriteLine($"{DateTime.Now}: Debug - Monster registered, reminder in {duration} second(s)");
        }

        private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Remove!!");
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            ToggleDisplay();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            player.controls.stop();
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            player.controls.play();
        }
    }
}
