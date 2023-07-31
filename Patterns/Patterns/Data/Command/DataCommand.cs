using CommunityToolkit.Diagnostics;
using Patterns.Command;
using Patterns.Data.Model;
using System.Collections.Generic;

namespace Patterns.Data.Command
{
    public abstract class DataCommand : IPatternzCommand
    {
        public CommandState State { get; set; }
        public DataFile? DataFile { get; protected set; }
        public ICollection<DataRecord>? RecordCollection { get; protected set;}
        public virtual string Name { get; } = string.Empty;

        public IPatternzCommandParam? Param { get; protected set; }

        protected virtual void CheckParamBeforeUnexecute()
        {
            if (Param == null)
            {
                ThrowHelper.ThrowInvalidOperationException("Cannot Unexecute command that has never been executed");
            }
        }

        public abstract void Unexecute();

        public abstract void Execute(IPatternzCommandParam? param = null);
    }
}
