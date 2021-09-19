using Microsoft.Win32;
using MonsterReminder.Controller;
using System;
using System.Text.Json;
using System.Windows;

namespace MonsterReminder.View
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

            LoadListSounds();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            InvalidateMeasure();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MonsterController.PlayOneRandomReminder();
        }

        private void Image_Click_AddSound(object sender, RoutedEventArgs e)
        {
            if (TextBoxSoundName.Text != "")
                ListViewSounds.Items.Add(TextBoxSoundName.Text);
        }

        private void Button_Click_RemoveSound(object sender, RoutedEventArgs e)
        {
            int index = ListViewSounds.SelectedIndex;

            if (index >= 0)
            {
                ListViewSounds.Items.RemoveAt(index);
            }
        }

        private void LoadListSounds()
        {
            foreach (string sound in MonsterController.Configuration.ReminderSounds)
            {
                ListViewSounds.Items.Add(sound);
            }
        }

        private void Image_Minimize_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine("Saving sounds list");
            MonsterController.Configuration.ReminderSounds.Clear();

            foreach (string item in ListViewSounds.Items)
            {
                MonsterController.AddReminderSound(item);
            }

            Close();
        }

        private void Image_Folder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBoxSoundName.Text = SelectFile();
        }

        private static string SelectFile()
        {
            OpenFileDialog openFileDialog = new();

            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;

            return "";
        }
    }
}
