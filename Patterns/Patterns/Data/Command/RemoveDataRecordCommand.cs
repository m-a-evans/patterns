﻿using CommunityToolkit.Diagnostics;
using Patterns.Command;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System.Collections.Generic;

namespace Patterns.Data.Command
{
    public class RemoveDataRecordCommand : DataCommand
    {
        public override string Name => nameof(RemoveDataRecordCommand);

        public RemoveDataRecordCommand(ICollection<DataRecord> receiver, RemoveDataRecordParam? param = null)
        {
            RecordCollection = receiver;
            Param = param;
        }

        public override void Execute(IPatternzCommandParam? param = null)
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
            State = CommandState.Executed;
        }

        public override void Unexecute()
        {
            CheckParamBeforeUnexecute();
            RecordCollection.Add(((RemoveDataRecordParam)Param).DataRecord);
            State = CommandState.Unexecuted;
        }
    }
}
