using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command
{
    public class EditDataRecordCommand : DataCommand
    {
        private DataRecord _previousState;
        private EditDataRecordParam _param;
        public override string CommandName => throw new NotImplementedException();

        public override DataCommandId Id => throw new NotImplementedException();

        public EditDataRecordCommand(DataFile receiver)
        {
            DataFile = receiver;
        }

        public override void Execute(IDataCommandParam? param = null)
        {
            param ??= _param;
            if (param is EditDataRecordParam setDataParam)
            {
                _previousState = DataFile.DataRecords[setDataParam.DataRecord.CreatedDate].DeepCopy();
                DataFile.DataRecords[setDataParam.DataRecord.CreatedDate] = setDataParam.DataRecord;
                _param = setDataParam;
            }
        }

        public override void Unexecute()
        {
            DataFile.DataRecords[_previousState.CreatedDate] = _previousState;
        }
    }
}
