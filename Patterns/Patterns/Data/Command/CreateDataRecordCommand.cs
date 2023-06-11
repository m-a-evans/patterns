using CommunityToolkit.Diagnostics;
using Patterns.Command;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System.Collections.Generic;
using System.Linq;

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
                State = CommandState.Executed;
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
            State = CommandState.Unexecuted;
            RecordCollection.Remove(((CreateDataRecordParam)Param).DataRecord);
            DataRecord record;
            for (int i = 0; i < RecordCollection.Count; i++)
            {
                record = RecordCollection.ElementAt(i);
                if (record.Id == ((CreateDataRecordParam)Param).DataRecord.Id)
                {
                    RecordCollection.Remove(record);
                    break;
                }
            }
        }
    } 
}
