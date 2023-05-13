using Patterns.Data.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsUI.Model
{
    internal class CommandHistoryMetadata
    {
        public string FileName { get; set; }
        public List<DataCommand> History { get; set; }
    }
}
