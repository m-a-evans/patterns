using Patterns.Command;
using Patterns.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command.Parameter
{
    public class RemoveDataRecordParam : IPatternzCommandParam
    {
        public string Name => nameof(RemoveDataRecordParam);

        public DataRecord DataRecord { get; private set; }

        public RemoveDataRecordParam(DataRecord dataRecord)
        {
            DataRecord = dataRecord;
        }
    }
}
