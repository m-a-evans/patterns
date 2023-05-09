using Patterns.Data.Command.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Patterns.Data.Command
{
    internal class CreateDataRecordCommand : IDataCommand
    {
        public string CommandName => nameof(CreateDataRecordCommand);

        public DataCommandId Id => DataCommandId.CreateDataRecord;

        public CreateDataRecordCommand(CreateDataRecordParam param)
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
