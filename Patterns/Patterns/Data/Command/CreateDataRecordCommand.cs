using CommunityToolkit.Diagnostics;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System.Collections.Generic;

namespace Patterns.Data.Command
{
    public class CreateDataRecordCommand : DataCommand
    {
        public override string CommandName => nameof(CreateDataRecordCommand);

        public override DataCommandId Id => DataCommandId.CreateDataRecord;

        public CreateDataRecordCommand(ICollection<DataRecord> receiver, CreateDataRecordParam? param = null)
        {
            RecordCollection = receiver;
            Param = param;
        }

        public override void Execute(IDataCommandParam? param = null)
        {
            param ??= Param;
            if (param is CreateDataRecordParam createDataParam)
            {
                Param = createDataParam;
                RecordCollection.Add(createDataParam.DataRecord);
            }
            else
            {
                ThrowHelper.ThrowArgumentException($"Param must be of type {nameof(CreateDataRecordParam)}");
            }
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            RecordCollection.Remove(((CreateDataRecordParam)Param).DataRecord);
        }
    } 
}
