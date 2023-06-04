using CommunityToolkit.Diagnostics;
using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;
using System.Collections;
using System.Collections.Generic;

namespace Patterns.Data.Command
{
    public abstract class DataCommand
    {
        public DataFile? DataFile { get; protected set; }
        public ICollection<DataRecord>? RecordCollection { get; protected set;}
        public virtual string CommandName { get; } = string.Empty;
        public virtual DataCommandId Id { get; protected set; }

        public IDataCommandParam? Param { get; protected set; }

        protected virtual void CheckParamBeforeUnexecute()
        {
            if (Param == null)
            {
                ThrowHelper.ThrowInvalidOperationException("Cannot Unexecute command that has never been executed");
            }
        }

        public abstract void Unexecute();

        public abstract void Execute(IDataCommandParam? param = null);
    }
}
