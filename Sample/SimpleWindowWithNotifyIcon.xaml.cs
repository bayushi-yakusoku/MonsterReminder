using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace MonsterReminder.Sample
{
    /// <summary>
    /// Interaction logic for SimpleWindowWithNotifyIcon.xaml
    /// </summary>
    public partial class SimpleWindowWithNotifyIcon : Window
    {
        Timer TimerMonster;

        Timer TimerSingleClick;

        public SimpleWindowWithNotifyIcon()
        {
            InitializeComponent();

            InitializeTimer();

            InitializeTimerSingleClick();
        }

        private void InitializeTimer()
        {
            TimerMonster = new Timer
            {
                Interval = 5000,
                AutoReset = false
            };

            TimerMonster.Elapsed += Timer_Elapsed;
        }

        private void InitializeTimerSingleClick()
        {
            TimerSingleClick = new Timer
            {
                Interval = 1000,
                AutoReset = false
            };

            TimerSingleClick.Elapsed += TimerSingleClick_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Ring!");
        }

        private void ButtonStartTimer_Click(object sender, RoutedEventArgs e)
        {
            TimerMonster.Start();
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

        /* **********************************************************
         *  EVENTS
         * ********************************************************** */

        int pouf = 0;
        private void MyNotifyIcon_LeftClick(object sender, RoutedEventArgs e)
        {
            pouf++;
            e.Handled = true;

            TimerSingleClick.Stop();
            TimerSingleClick.Start();
            
            Debug.WriteLine($"{DateTime.Now}: Debug - Single Click Event: pouf={pouf}");

        }

        private void TimerSingleClick_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Timer Single Click Elpased!");

            // Will be executed from a thread different from the UI Element owner
            this.Dispatcher.Invoke(() =>
            {
                ToggleDisplay();
            });
        }

        private void MyNotifyIcon_RightClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void ToggleDisplay()
        {
            int margin = 10;

            if (WindowState != WindowState.Minimized) 
            {
                WindowState = WindowState.Minimized;
                return;
            }

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;

            this.WindowState = WindowState.Normal;

            this.Left = desktopWorkingArea.Right - (this.Width + margin);
            this.Top = desktopWorkingArea.Bottom - (this.Height + margin);
        }

        private void MyNotifyIcon_DoubleClick(object sender, RoutedEventArgs e)
        {
            // preventing single click event to be fired after this double click
            //e.Handled = true;

            //Timer.Start();

            TimerSingleClick.Stop();

            Debug.WriteLine($"{DateTime.Now}: Debug - Double Click Event: pouf={pouf}");
        }

        private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"{DateTime.Now}: Debug - Remove!!");
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
