using CommunityToolkit.Diagnostics;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;

namespace Patterns.Data.Command
{
    public class SetDataRecordFormatCommand : DataCommand
    {
        private DataRecordFormat _previousState;

        public override string CommandName => nameof(SetDataRecordFormatCommand);

        public override DataCommandId Id => DataCommandId.SetDataRecordFormat;

        public SetDataRecordFormatCommand(DataFile receiver, SetDataRecordFormatParam? param = null)
        {
            DataFile = receiver;
            Param = param;
        }

        public override void Execute(IDataCommandParam? param = null)
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
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            DataFile.Format = _previousState;
        }
    }
}
