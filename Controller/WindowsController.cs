using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterReminder.Sample
{
    class WindowsController
    {
        private static Lazy<WindowsController> lazy = new(() => new());
        private static WindowsController Instance => lazy.Value;

        private WindowsController()
        {
            Debug.WriteLine($"WindowsController: Private Constructor");
        }

    }
}
