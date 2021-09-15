using System.Text.Json;
using System.Windows;

namespace MonsterReminder.Sample
{
    /// <summary>
    /// Interaction logic for ListSounds.xaml
    /// </summary>
    public partial class ListSounds : Window
    {
        readonly MonsterController MonsterController;

        public ListSounds()
        {
            InitializeComponent();

            MonsterController = MonsterController.Instance;

            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };

            TextBoxTest.Text = JsonSerializer.Serialize(MonsterController.Configuration, options);
        }
    }
}
