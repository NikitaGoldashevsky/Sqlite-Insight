namespace SQLite_Insight
{
    internal interface IDatabaseAction
    {
        void FillDataGrid();

        void SetSelectionButtonVisibility(bool state);
    }
}
