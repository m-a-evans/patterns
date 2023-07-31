using System;

namespace Patterns.Account.Model
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
