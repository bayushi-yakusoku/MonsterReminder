using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonsterReminder.Tool
{
    class Log
    {
        public static void Debug(string message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string member = "")
        {
            var s = string.Format("{0}:{1} - {2}: {3}", file, line, member, message);

            System.Diagnostics.Debug.WriteLine(s);
            Console.WriteLine(s);
        }
    }
}
