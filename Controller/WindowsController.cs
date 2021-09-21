using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MonsterReminder.View;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MonsterReminder.Tool;

namespace MonsterReminder.Controller
{
    class WindowsController
    {
        private static Lazy<WindowsController> lazy = new(() => new());
        public static WindowsController Instance => lazy.Value;

        private WindowsController()
        {
            Debug.WriteLine($"WindowsController: Private Constructor");
        }

        int margin = 10;

        WindowInfo WindowInfo;

        public void ShowWindowInfo()
        {
            Rect desktopWorkingArea = SystemParameters.WorkArea;
            
            if (WindowInfo == null)
                WindowInfo = new();

            WindowInfo.Closed += WindowInfo_Closed;

            WindowInfo.Show();

            WindowInfo.Left = desktopWorkingArea.Right - (WindowInfo.Width + margin);
            WindowInfo.Top = desktopWorkingArea.Bottom - (WindowInfo.Height + margin);
        }

        private void WindowInfo_Closed(object sender, EventArgs e)
        {
            Log.Debug("WindowInfo Closed event");
            WindowInfo = null;
        }

        ListSounds ListSounds;

        public void ShowListSounds()
        {
            Rect desktopWorkingArea = SystemParameters.WorkArea;

            ListSounds = new();
            ListSounds.Show();

            ListSounds.Left = desktopWorkingArea.Right - (ListSounds.Width + margin);
            ListSounds.Top = desktopWorkingArea.Bottom - (ListSounds.Height + margin);
        }
    }
}
