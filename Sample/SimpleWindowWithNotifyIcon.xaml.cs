using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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

namespace MonsterReminder.Sample
{
    /// <summary>
    /// Interaction logic for SimpleWindowWithNotifyIcon.xaml
    /// </summary>
    public partial class SimpleWindowWithNotifyIcon : Window
    {
        Timer Timer;

        public SimpleWindowWithNotifyIcon()
        {
            InitializeComponent();

            InitializeTimer();
        }

        private void InitializeTimer()
        {
            Timer = new Timer
            {
                Interval = 5000,
                AutoReset = false
            };

            Timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            MessageBox.Show("Ring!");
        }

        private void ButtonStartTimer_Click(object sender, RoutedEventArgs e)
        {
            Timer.Start();
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

        private void MyNotifyIcon_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            Timer.Start();
        }

        private void MyNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Double!!");
        }

        private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Remove!!");
        }
    }
}
