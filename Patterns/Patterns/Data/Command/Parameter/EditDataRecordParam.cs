using Patterns.Data.Model;

namespace Patterns.Data.Command.Parameter
{
    public class EditDataRecordParam : IDataCommandParam
    {
        public string Name => nameof(EditDataRecordParam);

        public string DataRecordName { get; set; }
        public DataRecord DataRecord { get; set; }

        public EditDataRecordParam(string dataRecordName)
        {
            DataRecordName = dataRecordName;
        }
    }
}
