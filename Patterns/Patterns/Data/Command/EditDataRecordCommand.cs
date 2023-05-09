using Patterns.Data.Command.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command
{
    internal class EditDataRecordCommand : IDataCommand
    {
        public string CommandName => throw new NotImplementedException();

        public DataCommandId Id => throw new NotImplementedException();

        public EditDataRecordCommand(EditDataRecordParam param)
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
