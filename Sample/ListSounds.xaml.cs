using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MonsterReminder.Sample
{
    /// <summary>
    /// Interaction logic for ListSounds.xaml
    /// </summary>
    public partial class ListSounds : Window
    {
        MonsterController MonsterController;

        public ListSounds()
        {
            InitializeComponent();

            MonsterController = MonsterController.Instance;

            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };

            TextBoxTest.Text = JsonSerializer.Serialize(MonsterController.Configuration);
        }
    }
}
