using Patterns.Data.Command.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command
{
    internal class EditDataFileCommand : IDataCommand
    {
        private DateTime _previousModified;
        private string _previousFileName;

        public string CommandName => throw new NotImplementedException();

        public DataCommandId Id => throw new NotImplementedException();

        public EditDataFileCommand(EditDataFileParam param)
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
