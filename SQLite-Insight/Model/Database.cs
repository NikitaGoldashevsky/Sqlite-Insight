using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace SQLite_Insight.Model
{
    internal partial class Database : ObservableObject
    {

        [ObservableProperty]
        ObservableCollection<DatabaseRow> rows;

        [ObservableProperty]
        string path;


        public Database(string path)
        {
            rows = new ObservableCollection<DatabaseRow>();
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
                        string tableName = reader.GetString(0);

                        // Read data from each table
                        var dataCommand = connection.CreateCommand();
                        dataCommand.CommandText = $"SELECT * FROM {tableName}";

                        using (var dataReader = dataCommand.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                var row = new DatabaseRow();
                                for (int i = 0; i < dataReader.FieldCount; i++)
                                {
                                    string columnName = dataReader.GetName(i);
                                    object value = dataReader.GetValue(i);
                                    row.Columns[columnName] = value;
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

    }
}
