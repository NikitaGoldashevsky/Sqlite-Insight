using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace SQLite_Insight.Model
{
    internal partial class Database : ObservableObject
    {

        [ObservableProperty]
        ObservableCollection<Dictionary<string, string>> rows;

        [ObservableProperty]
        private string? path;

        [ObservableProperty]
        private string? tableName;


        public Database(string path)
        {
            rows = new ObservableCollection<Dictionary<string, string>>();
            Path = path;
            LoadDatabaseContent();
        }


        public static bool IsValidSqliteDatabase(string databasePath)
        {
            if (!File.Exists(databasePath)) return false;

            try
            {
                using (var connection = new SqliteConnection($"Data Source={databasePath}"))
                {
                    connection.Open();
                    return true;
                }
            }
            catch (SqliteException)
            {
                return false;
            }
        }


        public ObservableCollection<string> GetColumnNames()
        {
            ObservableCollection<string> columnNames = new ObservableCollection<string>();

            using (var connection = new SqliteConnection($"Data Source={Path}"))
            {
                connection.Open();
                using (var command = new SqliteCommand($"PRAGMA table_info({TableName});", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // the column name is in the second column (index 1)
                            string columnName = reader.GetString(1);
                            columnNames.Add(columnName);
                        }
                    }
                }
            }
            
            return columnNames;
        }


        private void LoadDatabaseContent()
        {
            string connectionString = $"Data Source={Path}";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Get all table names
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";

                using (var reader = tableCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TableName = reader.GetString(0);

                        // Read data from each table
                        var dataCommand = connection.CreateCommand();
                        dataCommand.CommandText = $"SELECT * FROM {TableName}";

                        using (var dataReader = dataCommand.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                var row = new Dictionary<string, string>();
                                for (int i = 0; i < dataReader.FieldCount; i++)
                                {
                                    string columnName = dataReader.GetName(i);
                                    string value = (string)dataReader.GetValue(i).ToString();
                                    row[columnName] = value;
                                }
                                Rows.Add(row);
                            }
                        }
                    }
                }
            }
        }


        // CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL)
        public bool Execute(string query)
        {
            string sql = query;
            string connectionString = $"Data Source={Path}";

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update()
        {
            if (Path == null)
            {
                return false;
            }

            Rows = new ObservableCollection<Dictionary<string, string>>();
            LoadDatabaseContent();
            return true;
        }

        public bool DeleteRow(string column1Value)
        {
            using (var connection = new SqliteConnection("Data Source=mydatabase.db"))
            {
                connection.Open();
                

                var command = connection.CreateCommand();
                command.CommandText =
                $@"
                    DELETE FROM {tableName}
                    WHERE Id = $id;
                ";

                //// Use parameterized queries to prevent SQL Injection
                //command.Parameters.AddWithValue("$id", personId);

                // Execute the command
                int rowsAffected = command.ExecuteNonQuery(); // Returns the number of affected rows

                if (rowsAffected > 0)
                {
                    MessageBox.Show($"Row deleted successfully!");
                    return true;
                }
                else
                {
                    MessageBox.Show($"Row deletion failed.");
                    return false;
                }
            }
        }

    }
}
