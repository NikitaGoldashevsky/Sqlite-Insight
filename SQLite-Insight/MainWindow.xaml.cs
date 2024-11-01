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
            if (!(DataContext is MainWindowViewModel vm))
            {
                MessageBox.Show("Error while filling data grid.", "Error");
                return;
            }

            myDataGrid.Columns.Clear();

            if (vm.CurrentDatabase.SelectionMode == false)
            {
                var database = vm.CurrentDatabase;

                if (database.Rows.Count == 0)
                {
                    return;
                }

                foreach (var key in database.Rows[0].Keys)
                {
                    DataGridTextColumn col = new DataGridTextColumn
                    {
                        Header = key,
                        Binding = new Binding($"[{key}]")
                    };
                    myDataGrid.Columns.Add(col);
                }

                myDataGrid.ItemsSource = database.Rows;
            }
            else
            {
                var dataCollection = vm.CurrentDatabase.CurrentSelection;

                if (dataCollection.Count == 0)
                {
                    return;
                }

                foreach (var key in dataCollection[0].Keys)
                {
                    DataGridTextColumn col = new DataGridTextColumn
                    {
                        Header = key,
                        Binding = new Binding($"[{key}]")
                    };
                    myDataGrid.Columns.Add(col);
                }

                myDataGrid.ItemsSource = dataCollection;
            }
        }

    }
}
