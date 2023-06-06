using Patterns.Command;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;

namespace Patterns.Data.Command
{
    public class EditDataFileCommand : DataCommand
    {
        private DataFile _previousState;
        private EditDataFileParam _param;

        public override string Name => nameof(EditDataFileCommand);

        public EditDataFileCommand(DataFile receiver, EditDataFileParam? param = null)
        {
            DataFile = receiver;
            Param = param;
        }

        public override void Execute(IPatternzCommandParam? param = null)
        {
            param ??= _param;
            if (param is EditDataFileParam editDataParam)
            {
                _previousState = DataFile.DeepCopy();
                DataFile = editDataParam.DataFile;
                _param = editDataParam;
            }
            State = CommandState.Executed;
        }

        public override void Unexecute()
        {
            DataFile = _previousState;
            State = CommandState.Unexecuted;
        }
    }
}
