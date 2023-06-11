using Patterns.Command;
using Patterns.Data.Model;

namespace Patterns.Data.Command.Parameter
{
    public class CreateDataRecordParam : IPatternzCommandParam
    {
        public string Name => nameof(CreateDataRecordParam);

        public object Value { get => DataRecord; }

        public DataRecord DataRecord { get; private set; }

        public CreateDataRecordParam(DataRecord dataRecord)
        {
            DataRecord = dataRecord;
        }
    }
}
