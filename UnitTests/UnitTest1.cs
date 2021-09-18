using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MonsterReminder.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        static int Add(int x, int y)
        {
            return x + y;
        }

        [Fact]
        public void TestAddSong()
        {
            Box box = new();
            box.ListSongs.Add("pouf");
            box.ListSongs.Add("Wrecked");
            box.ListSongs.Add("pouf");

            Assert.Equal(2, box.ListSongs.Distinct().Count());

            Assert.Equal(3, box.ListSongs.Count);
        }

        [Fact]
        public void TestAddReminders()
        {
            Box box = new();
            box.ListReminders.Add("1", "pouf");
            box.ListReminders.Add("2", "pouf");

            Assert.Equal(2, box.ListReminders.Count);
        }

        [Fact]
        public void TestExistMethod()
        {
            List<string> Songs = new();

            Songs.Add("Wrecked");
            Songs.Add("Saint");

            string name = "Saint";
            Assert.True(Songs.Exists( s => s == name ));

            name = "pouf";
            Assert.False(Songs.Exists( s => s == name ));
        }
    }

    class Box
    {
        public readonly List<string> ListSongs = new();
        public readonly Dictionary<string, string> ListReminders = new();
    }
}
