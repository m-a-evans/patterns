using Patterns.Data.Model;

namespace Patterns.Data.Command.Parameter
{
    public class EditDataRecordParam : IDataCommandParam
    {
        public string Name => nameof(EditDataRecordParam);
        public DataRecord DataRecord { get; set; }

        public EditDataRecordParam(DataRecord dataRecord)
        {
            DataRecord = dataRecord;
        }
    }
}
