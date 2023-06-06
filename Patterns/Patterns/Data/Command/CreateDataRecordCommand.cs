using CommunityToolkit.Diagnostics;
using Patterns.Command;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System.Collections.Generic;

namespace Patterns.Data.Command
{
    public class CreateDataRecordCommand : DataCommand
    {
        public override string Name => nameof(CreateDataRecordCommand);
        public CreateDataRecordCommand(ICollection<DataRecord> receiver, CreateDataRecordParam? param = null)
        {
            RecordCollection = receiver;
            Param = param;
        }

        public override void Execute(IPatternzCommandParam? param = null)
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
            State = CommandState.Executed;
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            RecordCollection.Remove(((CreateDataRecordParam)Param).DataRecord);
            State = CommandState.Unexecuted;
        }
    } 
}
