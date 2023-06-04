using CommunityToolkit.Diagnostics;
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
        public override string CommandName => nameof(RemoveDataRecordCommand);

        public override DataCommandId Id => DataCommandId.RemoveDataRecord;

        public RemoveDataRecordCommand(ICollection<DataRecord> receiver, RemoveDataRecordParam? param = null)
        {
            RecordCollection = receiver;
            Param = param;
        }

        public override void Execute(IDataCommandParam? param = null)
        {
            param ??= Param;
            if (param is RemoveDataRecordParam removeDataParam)
            {
                Param = removeDataParam;
                RecordCollection.Remove(removeDataParam.DataRecord);
            }
            else
            {
                ThrowHelper.ThrowArgumentException($"Param must be of type {nameof(RemoveDataRecordParam)}");
            }
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            RecordCollection.Add(((RemoveDataRecordParam)Param).DataRecord);
        }
    }
}
