using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite_Insight
{
    internal interface IDatabaseAction
    {
        void FillDataGrid();

        void SetSelectionButtonVisibility(bool state);
    }
}
