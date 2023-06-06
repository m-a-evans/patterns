namespace Patterns.Command
{
    public interface IPatternzCommand
    {
        public CommandState State { get; set; }
        public void Execute(IPatternzCommandParam? param = null);

        public void Unexecute();
    }
}
