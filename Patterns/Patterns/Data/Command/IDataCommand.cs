using Patterns.Data.Command.Parameter;

namespace Patterns.Data.Command
{
    internal interface IDataCommand
    {
        string CommandName { get; }
        DataCommandId Id { get; }

        void Unexecute();

        void Execute();
    }
}
