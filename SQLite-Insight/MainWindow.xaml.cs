using SQLite_Insight.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

                if (database.Rows.Count == 0)
                {
                    return;
                }

                // Set up columns
                foreach (var key in database.Rows[0].Keys)
                {
                    DataGridTextColumn col = new DataGridTextColumn
                    {
                        Header = key,
                        Binding = new Binding($"[{key}]")
                    };
                    myDataGrid.Columns.Add(col);
                }

                // Set ItemsSource
                myDataGrid.ItemsSource = database.Rows;
            }
            else
            {
                MessageBox.Show("Error while filling data grid.", "Error");
            }
        }

    }
}
