using CommunityToolkit.Diagnostics;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace Patterns.Data.Command
{
    public class EditDataRecordCommand : DataCommand
    {
        private DataRecord? _previousState;
        public override string CommandName => nameof(EditDataRecordCommand);

        public override DataCommandId Id => DataCommandId.EditDataRecord;

        public EditDataRecordCommand(ICollection<DataRecord> receiver, EditDataRecordParam? param = null)
        {
            RecordCollection = receiver;
            Param = param;
        }

        public override void Execute(IDataCommandParam? param = null)
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
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            _ = ReplaceRecordInCollection(_previousState!);
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
