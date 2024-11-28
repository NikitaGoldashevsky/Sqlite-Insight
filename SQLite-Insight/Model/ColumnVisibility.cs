using CommunityToolkit.Mvvm.ComponentModel;

namespace SQLite_Insight.Model
{
    internal partial class ColumnVisibility : ObservableObject
    {
        public ColumnVisibility(string _header, bool _isVisible)
        {
            Header = _header;
            IsVisible = _isVisible;
        }

        [ObservableProperty]
        private string header;

        [ObservableProperty]
        private bool isVisible;
    }
}
