using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SQLite_Insight.View
{
    public partial class SelectDialog: Window
    {
        public List<string> Options { get; private set; }
        public string SelectedOption { get; private set; }

        public SelectDialog(string title, string query, List<string> _options)
        {
            InitializeComponent();
            Title = title;
            QueryLabel.Content = query;

            Options = _options;
            foreach (string option in Options)
            {
                OptionsComboBox.Items.Add(option);
            }
            OptionsComboBox.SelectedIndex = 0;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsComboBox.SelectedItem != null)
            {
                SelectedOption = OptionsComboBox.SelectedItem.ToString();
                DialogResult = true;
                Close();
            }
        }
    }
}
