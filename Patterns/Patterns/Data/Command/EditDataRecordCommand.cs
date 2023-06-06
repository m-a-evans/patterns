using CommunityToolkit.Diagnostics;
using Patterns.Command;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Patterns.Data.Command
{
    public class EditDataRecordCommand : DataCommand
    {
        private DataRecord? _previousState;
        public override string Name => nameof(EditDataRecordCommand);

        public EditDataRecordCommand(ICollection<DataRecord> receiver, EditDataRecordParam? param = null)
        {
            RecordCollection = receiver;
            Param = param;
            if (param?.PreviousState != null) 
            {
                _previousState = param.PreviousState;
            }
        }

        public override void Execute(IPatternzCommandParam? param = null)
        {
            param ??= Param;
            if (param is EditDataRecordParam setDataParam)
            { 
                Param = setDataParam;
                _previousState = ReplaceRecordInCollection(setDataParam.DataRecord);
            }
            else
            {
                ThrowHelper.ThrowArgumentException($"Param must be of type {nameof(EditDataRecordParam)}");
            }
            State = CommandState.Executed;
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            _ = ReplaceRecordInCollection(_previousState!);
            State = CommandState.Unexecuted;
        }

        private DataRecord ReplaceRecordInCollection(DataRecord newRecord)
        {
            DataRecord? retVal = null;
            for (int i = 0; i < RecordCollection.Count; i++)
            {
                DataRecord record = RecordCollection.ElementAt(i);
                if (record.Id == newRecord.Id)
                {
                    retVal = RecordCollection.ElementAt(i).DeepCopy();
                    record = newRecord;
                    break;
                }
            }
            if (retVal == null) 
            {
                ThrowHelper.ThrowInvalidOperationException<DataRecord>($"{nameof(DataRecord)} does not exist in collection");
            }
            return retVal!;
        }
    }
}
