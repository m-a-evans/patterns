using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command
{
    public class RemoveDataRecordCommand : DataCommand
    {
        private DataRecord _previousState;
        private RemoveDataRecordParam _param;
        public override string CommandName => nameof(RemoveDataRecordCommand);

        public override DataCommandId Id => DataCommandId.RemoveDataRecord;

        public RemoveDataRecordCommand(DataFile receiver)
        {
            DataFile = receiver;
        }

        public override void Execute(IDataCommandParam? param = null)
        {
            param ??= _param;
            if (param is RemoveDataRecordParam removeDataParam)
            {
                _previousState = DataFile.DataRecords[removeDataParam.DataRecord.CreatedDate].DeepCopy();
                DataFile.DataRecords.Remove(removeDataParam.DataRecord.CreatedDate);
                _param = removeDataParam;
            }
        }

        public override void Unexecute()
        {
            DataFile.DataRecords.Add(_previousState.CreatedDate, _previousState);
        }
    }
}
