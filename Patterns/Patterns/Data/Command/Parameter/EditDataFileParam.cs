using Patterns.Command;
using Patterns.Data.Model;

namespace Patterns.Data.Command.Parameter
{
    public class EditDataFileParam : IPatternzCommandParam
    {
        public string Name => nameof(EditDataFileParam);

        public DataFile DataFile { get; private set; }
        
        public EditDataFileParam(DataFile dataFile) 
        {
            DataFile = dataFile;
        }
    }
}
