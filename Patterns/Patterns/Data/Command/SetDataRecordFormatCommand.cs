using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;

namespace Patterns.Data.Command
{
    public class SetDataRecordFormatCommand : DataCommand
    {
        private DataRecordFormat _previousState;
        private SetDataRecordFormatParam _param;

        public override string CommandName => "Set Data Record Format";

        public override DataCommandId Id => DataCommandId.SetDataRecordFormat;

        public SetDataRecordFormatCommand(DataFile receiver)
        {
            DataFile = receiver;
        }

        public override void Execute(IDataCommandParam? param = null)
        {
            param ??= _param;
            if (param is SetDataRecordFormatParam formatParam)
            {
                _previousState = DataFile.Format;
                DataFile.Format = formatParam.Format;
                _param = formatParam;
            }
        }

        public override void Unexecute()
        {
            DataFile.Format = _previousState;
        }
    }
}
