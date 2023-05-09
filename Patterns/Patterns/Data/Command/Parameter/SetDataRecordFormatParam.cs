using Patterns.Data.Model;

namespace Patterns.Data.Command.Parameter
{
    public class SetDataRecordFormatParam : IDataCommandParam
    {
        public string Name => nameof(SetDataRecordFormatParam);

        public DataRecordFormat Format { get; private set; }

        public SetDataRecordFormatParam(DataRecordFormat format)
        {
            Format = format;
        }
    }
}
