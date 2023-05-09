using Patterns.Data.Command.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command
{
    internal class SetDataRecordFormatCommand : IDataCommand
    {
        public string CommandName => "Set Data Record Format";

        public DataCommandId Id => DataCommandId.SetDataRecordFormat;

        public SetDataRecordFormatCommand(SetDataRecordFormatParam param)
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
