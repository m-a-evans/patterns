using Patterns.Data.Command.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command
{
    internal class RemoveDataRecordCommand : IDataCommand
    {
        public string CommandName => nameof(RemoveDataRecordCommand);

        public DataCommandId Id => DataCommandId.RemoveDataRecord;

        public RemoveDataRecordCommand(RemoveDataRecordParam param)
        {

        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Unexecute()
        {
            throw new NotImplementedException();
        }
    }
}
