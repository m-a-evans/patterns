using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Account
{
    [Flags]
    public enum Permission : ulong
    {
        None = 0,
        AddUser = 1,
        RemoveUser = 2,
        UpdateUser = 4,
        WriteAccess = 8,
        ReadAccess = 16,
        // Other permissions here...
    }
}
