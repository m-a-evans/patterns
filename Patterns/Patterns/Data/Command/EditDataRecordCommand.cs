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
                State = CommandState.Executed;
                Param = setDataParam;
                _previousState = ReplaceRecordInCollection(setDataParam.DataRecord);
            }
            else
            {
                ThrowHelper.ThrowArgumentException($"Param must be of type {nameof(EditDataRecordParam)}");
            }            
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            State = CommandState.Unexecuted;
            _ = ReplaceRecordInCollection(_previousState!);            
        }

        private DataRecord ReplaceRecordInCollection(DataRecord newRecord)
        {
            DataRecord? retVal = null;
            DataRecord record;
            for (int i = 0; i < RecordCollection.Count; i++)
            {
                record = RecordCollection.ElementAt(i);
                if (record.Id == newRecord.Id)
                {
                    retVal = RecordCollection.ElementAt(i).DeepCopy();
                    if (RecordCollection is IList<DataRecord> list)
                    {
                        list.RemoveAt(i);
                        list.Insert(i, newRecord.DeepCopy());
                    }
                    else
                    {
                        RecordCollection.Remove(record);
                        RecordCollection.Add(newRecord.DeepCopy());
                    }
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
