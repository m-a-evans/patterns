using Patterns.Data.Command.Parameter;
using Patterns.Data.Model;

namespace Patterns.Data.Command
{
    public abstract class DataCommand
    {
        protected DataFile DataFile { get; set; }
        public virtual string CommandName { get; } = string.Empty;
        public virtual DataCommandId Id { get; protected set; }

        public abstract void Unexecute();

        public abstract void Execute(IDataCommandParam? param = null);
    }
}
