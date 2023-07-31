using Patterns.Command;
using Patterns.Data.Model;

namespace Patterns.Data.Command.Parameter
{
    public class EditDataRecordParam : IPatternzCommandParam
    {
        public string Name => nameof(EditDataRecordParam);

        public object Value { get => DataRecord; }
        public DataRecord DataRecord { get; set; }
        public DataRecord? PreviousState { get; set; }

        public EditDataRecordParam(DataRecord newState, DataRecord? previousState = null)
        {
            DataRecord = newState.DeepCopy();
            PreviousState = previousState?.DeepCopy();
        }
    }
}
