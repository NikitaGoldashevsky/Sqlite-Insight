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
using SQLite_Insight.View;
using System.Linq;

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
        public RelayCommand QueryAddCommand { get; }
        public RelayCommand QueryDropCommand { get; }
        public RelayCommand QueryRenameCommand { get; }

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
            QueryAddCommand = new RelayCommand(OnQueryAdd);
            QueryDropCommand = new RelayCommand(OnQueryDrop);
            QueryRenameCommand = new RelayCommand(OnQueryRename);

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
                try
                {
                    CurrentDatabase.Execute(QueryTextBoxContent);

                    CurrentDatabase.Update();
                    this.databaseAction.FillDataGrid();
                    return;
                }
                catch (Exception ex) { 
                    errorMessage = $"Query execution failed: {ex.Message}";
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
                string columnNamesString = string.Join(", ", CurrentDatabase.GetColumnNames());
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
                string columnNamesString = string.Join(", ", CurrentDatabase.GetColumnNames());
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
                QueryTextBoxContent = $"DELETE FROM {CurrentDatabase.TableName} WHERE (  );";
            }
            else
            {
                QueryTextBoxContent = sampleSelectQuery;
            }
        }


        private void OnQueryAdd()
        {
            string sampleSelectQuery = "ALTER TABLE table_name ADD column_name datatype;";

            if (CurrentDatabase != null)
            {
                QueryTextBoxContent = $"ALTER TABLE {CurrentDatabase.TableName} ADD name type;";
            }
            else
            {
                QueryTextBoxContent = sampleSelectQuery;
            }
        }


        private void OnQueryDrop()
        {
            string sampleSelectQuery = "ALTER TABLE table_name DROP COLUMN column_name;";

            if (CurrentDatabase != null)
            {
                QueryTextBoxContent = $"ALTER TABLE {CurrentDatabase.TableName} DROP COLUMN name;";
            }
            else
            {
                QueryTextBoxContent = sampleSelectQuery;
            }
        }


        private void OnQueryRename()
        {
            string sampleSelectQuery = "ALTER TABLE table_name RENAME COLUMN old_name TO new_name;";

            if (CurrentDatabase != null)
            {
                QueryTextBoxContent = $"ALTER TABLE {CurrentDatabase.TableName} RENAME COLUMN old TO new;";
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

            string? dialogResult;
            string tableName;
            do
            {
                dialogResult = InputDialogStatic.ShowDialog("Table name", "Name of a new table");
                if (dialogResult == null)
                {
                    return;
                }
            }
            while (dialogResult.IndexOf(' ') != -1);

            tableName = dialogResult;

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

                using (FileStream fs = File.Create(filePath)) { };

                CurrentDatabase = new Database(filePath, true, tableName);
            }
        }
    }
}
