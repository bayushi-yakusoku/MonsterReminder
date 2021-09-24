using MonsterReminder.Controller;
using MonsterReminder.Tools;
using System;
using System.Windows;

namespace MonsterReminder.View
{
    /// <summary>
    /// Interaction logic for ListSounds.xaml
    /// </summary>
    public partial class ListSounds : Window
    {
        private readonly MonsterController monsterController;

        public ListSounds()
        {
            InitializeComponent();

            monsterController = MonsterController.Instance;

            LoadListSounds();
        }

        private void LoadListSounds()
        {
            foreach (string sound in monsterController.Configuration.ReminderSounds)
            {
                ListViewSounds.Items.Add(sound);
            }
        }

        private void SavingSoundList()
        {
            Log.Debug("Saving sounds list");

            monsterController.Configuration.ReminderSounds.Clear();

            foreach (string item in ListViewSounds.Items)
            {
                monsterController.AddReminderSound(item);
            }
        }

        private void RemoveSelectedSound()
        {
            int index = ListViewSounds.SelectedIndex;

            if (index >= 0)
            {
                ListViewSounds.Items.RemoveAt(index);
            }
        }

        /* **********************************************************
         *  EVENTS
         * ********************************************************** */

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            InvalidateMeasure();
        }

        private void Image_Click_AddSound(object sender, RoutedEventArgs e)
        {
            if (TextBoxSoundName.Text != "")
            {
                ListViewSounds.Items.Add(TextBoxSoundName.Text);
            }
        }

        private void Button_Click_RemoveSound(object sender, RoutedEventArgs e)
        {
            RemoveSelectedSound();
        }

        private void ImageClose_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SavingSoundList();

            Close();
        }

        private void Image_Folder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBoxSoundName.Text = Tool.SelectFile();
        }
    }
}
