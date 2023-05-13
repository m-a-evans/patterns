using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System;

namespace Patterns.Data.Command
{
    public class CreateDataRecordCommand : DataCommand
    {
        private DateTime _previousState;

        private CreateDataRecordParam _param;

        public override string CommandName => nameof(CreateDataRecordCommand);

        public override DataCommandId Id => DataCommandId.CreateDataRecord;

        public CreateDataRecordCommand(DataFile receiver)
        {
            DataFile = receiver;
        }

        public override void Execute(IDataCommandParam? param = null)
        {
            param ??= _param;
            if (param is CreateDataRecordParam createDataParam)
            {
                DataFile.DataRecords.Add(createDataParam.DataRecord.CreatedDate, createDataParam.DataRecord);
                _previousState = createDataParam.DataRecord.CreatedDate;
                _param = createDataParam;
            }
        }

        public override void Unexecute()
        {
            DataFile.DataRecords.Remove(_previousState);
        }
    } 
}
