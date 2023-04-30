namespace Patterns.Account.Model
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
