using Microsoft.Win32;
using System;
using System.Runtime.CompilerServices;

namespace MonsterReminder.Tools
{
    class Tool
    {
        public static string SelectFile()
        {
            OpenFileDialog openFileDialog = new();

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return "";
        }
    }

    class Log
    {
        public static void Debug(string message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            string s = $"{DateTime.Now} - {file}:{line} - {member}: {message}";

            System.Diagnostics.Debug.WriteLine(s);
            //Console.WriteLine(s);
        }
    }
}
