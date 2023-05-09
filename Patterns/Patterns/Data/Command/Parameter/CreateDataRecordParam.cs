using Patterns.Data.Model;

namespace Patterns.Data.Command.Parameter
{
    public class CreateDataRecordParam : IDataCommandParam
    {
        public string Name => nameof(CreateDataRecordParam);

        public DataRecord DataRecord { get; private set; }

        public CreateDataRecordParam(DataRecord dataRecord)
        {
            DataRecord = dataRecord;
        }
    }
}
