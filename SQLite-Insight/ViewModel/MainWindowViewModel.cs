using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SQLite_Insight.Model;
using System.Windows;

namespace SQLite_Insight.ViewModel
{
    internal partial class MainWindowViewModel : ObservableObject
    {

        public RelayCommand OpenFileCommand { get; }
        public RelayCommand ClearQueryCommand { get; }
        public RelayCommand ExecuteQueryCommand { get; }

        [ObservableProperty]
        private string queryTextBoxContent;

        [ObservableProperty]
        public Database? currentDatabase;

        private readonly IDatabaseAction databaseAction;

        public MainWindowViewModel(IDatabaseAction databaseAction)
        {
            OpenFileCommand = new RelayCommand(OnOpenFile);
            ClearQueryCommand = new RelayCommand(OnClearQuery);
            ExecuteQueryCommand = new RelayCommand(OnExecuteQuery);

            this.databaseAction = databaseAction;
        }


        private void OnOpenFile()
        {
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = "SQLite Database | *.sqlite; *.sqlite3; *.db; *.db3; *.s3db; *.sl3";
            fileDialog.Title = "Pick an SQLite database file...";

            bool? opened = fileDialog.ShowDialog();
            if (opened == true)
            {
                string path = fileDialog.FileName;
                string fileName = fileDialog.SafeFileName;

                if (!Database.IsValidSqliteDatabase(path))
                {
                    return;
                };

                currentDatabase = new Database(path);
            }

            this.databaseAction.FillDataGrid();
        }


        private void OnExecuteQuery()
        {
            string errorMessage;

            if (CurrentDatabase != null)
            {
                if (CurrentDatabase.Execute(QueryTextBoxContent))
                {
                    return;
                }
                else
                {
                    errorMessage = "Query execution failed!";
                }
            }
            else
            {
                errorMessage = "No database opened!";
            }

            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        private void OnClearQuery()
        {
            QueryTextBoxContent = "";
        }
    }
}
