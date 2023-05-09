using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command
{
    internal enum DataCommandId
    {
        CreateDataRecord,
        RemoveDataRecord,
        EditDataFile,
        EditDataRecord,
        SetDataRecordFormat
    }
}
