using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Command
{
    public class EditDataFileCommand : DataCommand
    {
        private DataFile _previousState;
        private EditDataFileParam _param;

        public override string CommandName => throw new NotImplementedException();

        public override DataCommandId Id => throw new NotImplementedException();

        public EditDataFileCommand(DataFile receiver)
        {
            DataFile = receiver;
        }

        public override void Execute(IDataCommandParam? param = null)
        {
            param ??= _param;
            if (param is EditDataFileParam editDataParam)
            {
                _previousState = DataFile.DeepCopy();
                DataFile = editDataParam.DataFile;
                _param = editDataParam;
            }
        }

        public override void Unexecute()
        {
            DataFile = _previousState;
        }
    }
}
