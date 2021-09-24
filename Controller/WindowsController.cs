using System;
using MonsterReminder.View;
using System.Windows;
using MonsterReminder.Tools;

namespace MonsterReminder.Controller
{
    class WindowsController
    {
        private static readonly Lazy<WindowsController> lazy = new(() => new());
        public static WindowsController Instance => lazy.Value;

        private WindowsController()
        {
            Log.Debug("Private Constructor");
        }

        private readonly int margin = 10;
        private WindowInfo windowInfo;
        private ListSounds listSounds;

        public void ShowWindowInfo()
        {
            Rect desktopWorkingArea = SystemParameters.WorkArea;
            
            if (windowInfo == null)
            {
                windowInfo = new();
            }

            windowInfo.Closed += WindowInfo_Closed;

            windowInfo.WindowState = WindowState.Normal;

            windowInfo.Show();

            windowInfo.Left = desktopWorkingArea.Right - (windowInfo.Width + margin);
            windowInfo.Top = desktopWorkingArea.Bottom - (windowInfo.Height + margin);
        }

        private void WindowInfo_Closed(object sender, EventArgs e)
        {
            Log.Debug("Event");
            windowInfo = null;
        }

        public void ShowListSounds()
        {
            Rect desktopWorkingArea = SystemParameters.WorkArea;

            if (listSounds == null)
            {
                listSounds = new();
            }

            listSounds.Closed += ListSounds_Closed;

            listSounds.WindowState = WindowState.Normal;

            listSounds.Show();

            listSounds.Left = desktopWorkingArea.Right - (listSounds.Width + margin);
            listSounds.Top = desktopWorkingArea.Bottom - (listSounds.Height + margin);
        }

        private void ListSounds_Closed(object sender, EventArgs e)
        {
            Log.Debug("Event");
            listSounds = null;
        }
    }
}
