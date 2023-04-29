using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternsUI.MVVM.Messages
{
    public interface IMessage
    {
        static string Name { get; } = string.Empty;
    }
}
