﻿using System.Windows;

namespace SQLite_Insight.View
{

    public partial class InputDialog : Window
    {
        public string InputText { get; private set; }

        public InputDialog(string title, string query)
        {
            InitializeComponent();
            Title = title;
            QueryLabel.Content = query;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = InputTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
