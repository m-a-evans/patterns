using Patterns.Data.Model;

namespace Patterns.Data.Command.Parameter
{
    public class EditDataFileParam : IDataCommandParam
    {
        public string Name => nameof(EditDataFileParam);

        public DataFile DataFile { get; private set; }
        
        public EditDataFileParam(DataFile dataFile) 
        {
            DataFile = dataFile;
        }
    }
}
