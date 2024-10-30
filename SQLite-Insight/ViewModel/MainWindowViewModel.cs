using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using SQLite_Insight.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Windows;

namespace SQLite_Insight.ViewModel
{
    internal partial class MainWindowViewModel : ObservableObject
    {
        private const string mainWindowDefaultTitle = "SQLite-Insight";

        [ObservableProperty]
        private string mainWindowTitle = mainWindowDefaultTitle;

        public RelayCommand OpenFileCommand { get; }
        public RelayCommand NewFileCommand { get; }
        public RelayCommand ClearQueryCommand { get; }
        public RelayCommand ExecuteQueryCommand { get; }
        public RelayCommand HelpCommand { get; }
        public RelayCommand QueryInsertCommand { get; }
        public RelayCommand QueryDeleteCommand { get; }
        public RelayCommand QuerySelectCommand { get; }

        [ObservableProperty]
        private string queryTextBoxContent;

        [ObservableProperty]
        public Database? currentDatabase;

        private readonly IDatabaseAction databaseAction;


        public MainWindowViewModel(IDatabaseAction databaseAction)
        {
            OpenFileCommand = new RelayCommand(OnOpenFile);
            NewFileCommand = new RelayCommand(OnNewFile);
            ClearQueryCommand = new RelayCommand(OnClearQuery);
            ExecuteQueryCommand = new RelayCommand(OnExecuteQuery);
            HelpCommand = new RelayCommand(OnHelp);
            QueryInsertCommand = new RelayCommand(OnQueryInsert);
            QueryDeleteCommand = new RelayCommand(OnQueryDelete);
            QuerySelectCommand = new RelayCommand(OnQuerySelect);

            this.databaseAction = databaseAction;
        }


        private void OnOpenFile()
        {
            var fileDialog = new OpenFileDialog();

            fileDialog.Filter = "SQLite Database | *.sqlite; *.sqlite3; *.db; *.db3; *.s3db; *.sl3";
            fileDialog.Title = "Pick an SQLite database file";

            bool? opened = fileDialog.ShowDialog();
            if (opened == true)
            {
                string path = fileDialog.FileName;
                string fileName = fileDialog.SafeFileName;

                if (!Database.IsValidSqliteDatabase(path))
                {
                    return;
                };

                CurrentDatabase = new Database(path);
                this.databaseAction.FillDataGrid();
                MainWindowTitle = currentDatabase.TableName + " - " + mainWindowDefaultTitle;
            }
        }


        private void OnExecuteQuery()
        {
            string errorMessage;

            if (CurrentDatabase != null)
            {
                if (CurrentDatabase.Execute(QueryTextBoxContent))
                {
                    CurrentDatabase.Update();
                    this.databaseAction.FillDataGrid();
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


        private void OnHelp()
        {
            string url = "https://www.sqlitetutorial.net/";

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while opening webpage: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void OnQueryInsert()
        {
            string sampleInsertQuery = "INSERT INTO table_name () VALUES ();";

            if (CurrentDatabase != null)
            {
                string columnNamesString = string.Join(", ", currentDatabase.GetColumnNames());
                QueryTextBoxContent = $"INSERT INTO {CurrentDatabase.TableName} ({columnNamesString}) VALUES (  );";
            }
            else {
                QueryTextBoxContent = sampleInsertQuery;
            }
        }


        private void OnQuerySelect()
        {
            string sampleSelectQuery = "SELECT () FROM table_name;";

            if (CurrentDatabase != null)
            {
                string columnNamesString = string.Join(", ", currentDatabase.GetColumnNames());
                QueryTextBoxContent = $"SELECT {columnNamesString} FROM {CurrentDatabase.TableName};";
            }
            else
            {
                QueryTextBoxContent = sampleSelectQuery;
            }
        }


        private void OnQueryDelete()
        {
            string sampleSelectQuery = "DELETE FROM table_name WHERE condition;";

            if (CurrentDatabase != null)
            {
                string columnNames = "";
                foreach (string column in CurrentDatabase.GetColumnNames())
                {
                    columnNames += column;
                }
                QueryTextBoxContent = $"DELETE FROM {CurrentDatabase.TableName} WHERE (  );";
            }
            else
            {
                QueryTextBoxContent = sampleSelectQuery;
            }
        }


        private void OnNewFile()
        {
            SaveFileDialog fileDialog = new SaveFileDialog
            {
                Title = "Create a new database file",
                Filter = "SQLite Database (*.db)|*.db",
                DefaultExt = "db"
            };

            bool? fileCreated = fileDialog.ShowDialog();

            if (fileCreated == true)
            {
                string filePath = fileDialog.FileName;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (FileStream fs = File.Create(filePath)) { }
                string tableName = "test_table_name";
                currentDatabase = new Database(filePath, true, tableName);

                //MessageBox.Show($"File {System.IO.Path.GetFileName(filePath)} already exists!", 
                //    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
