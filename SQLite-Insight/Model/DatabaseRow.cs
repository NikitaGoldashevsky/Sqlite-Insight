using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace SQLite_Insight.Model
{
    internal partial class DatabaseRow : ObservableObject
    {
        [ObservableProperty]
        Dictionary<string, object>? columns;

        public DatabaseRow()
        {
            Columns = new Dictionary<string, object>();
        }
    }
}