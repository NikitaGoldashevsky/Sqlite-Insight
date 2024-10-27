using CommunityToolkit.Mvvm.ComponentModel;
using SQLite_Insight.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SQLite_Insight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDatabaseAction
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainWindowViewModel(this);
            DataContext = vm;
        }

        public void FillDataGrid()
        {
            if (DataContext is MainWindowViewModel vm)
            {
                var database = vm.CurrentDatabase;

                myDataGrid.Columns.Clear();

                // Set up columns
                foreach (var key in database.Rows[0].Keys)
                {
                    DataGridTextColumn col = new DataGridTextColumn
                    {
                        Header = key,
                        Binding = new Binding($"[{key}]") // Use indexer syntax for Dictionary
                    };
                    myDataGrid.Columns.Add(col);
                }

                // Set ItemsSource
                var a = database.Rows;
                var b = new ObservableCollection<Dictionary<string, string>> {
                    new Dictionary<string, string>{ {"id", "1" }, { "name", "alice" }, { "age", "30"} },
                    new Dictionary<string, string>{ { "id", "2"}, { "name", "john"}, { "age", "40"} }
                };
                myDataGrid.ItemsSource = a;
            }
            else
            {
                MessageBox.Show("DataContext is not of type MainWindowViewModel.");
            }
        }

    }
}
