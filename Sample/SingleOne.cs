using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterReminder.Sample
{
    class SingleOne
    {
        private static readonly Lazy<SingleOne> lazy = new(() => new());
        public static SingleOne Instance => lazy.Value;
        private SingleOne()
        {
#if DEBUG
            Debug.WriteLine("private constructor!");
#endif
            ReminderSounds = new();
        }

        public List<string> ReminderSounds { get; private set; }

        public string Name { get; set; }
    }
}
