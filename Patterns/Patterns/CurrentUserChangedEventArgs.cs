using Patterns.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    public class CurrentUserChangedEventArgs
    {
        public IPatternzUser OldUser { get; private set; }
        public IPatternzUser NewUser { get; private set; }

        public CurrentUserChangedEventArgs(IPatternzUser oldUser, IPatternzUser newUser)
        {
            OldUser = oldUser;
            NewUser = newUser;
        }
    }
}
