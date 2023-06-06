using CommunityToolkit.Diagnostics;
using Patterns.Command;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;

namespace Patterns.Data.Command
{
    public class SetDataRecordFormatCommand : DataCommand
    {
        private DataRecordFormat _previousState;

        public override string Name => nameof(SetDataRecordFormatCommand);

        public SetDataRecordFormatCommand(DataFile receiver, SetDataRecordFormatParam? param = null)
        {
            DataFile = receiver;
            Param = param;
        }

        public override void Execute(IPatternzCommandParam? param = null)
        {
            param ??= Param;
            if (param is SetDataRecordFormatParam formatParam)
            {
                Param = formatParam;
                _previousState = DataFile.Format;
                DataFile.Format = formatParam.Format;                
            }
            else
            {
                ThrowHelper.ThrowArgumentException($"Param must be of type {nameof(CreateDataRecordParam)}");
            }
            State = CommandState.Executed;
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            DataFile.Format = _previousState;
            State = CommandState.Unexecuted;
        }
    }
}
