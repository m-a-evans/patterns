namespace Patterns.Account.Model
{
    /// <summary>
    /// Concrete implementation of the IPatternzUser interface
    /// </summary>
    internal class PatternzUser : IPatternzUser
    {
        /// <summary>
        /// A flag indicating "any old user"
        /// </summary>
        public bool IsAnyUser { get; private set; }

        /// <summary>
        /// Gets the "AnyUser" object
        /// </summary>
        public static IPatternzUser AnyUser { get; } = new PatternzUser();

        /// <summary>
        /// Instantiates a new instance of PatternzUser
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="passwordHash">Hash of the password of the user</param>
        /// <param name="displayName">Optional. The in-app displayed name</param>
        /// <param name="permissions">Optional. What permissions this user has. Defaults to None</param>
        public PatternzUser(string username, string passwordHash, Permission permissions = Permission.None, string? displayName = null, string? pictureUrl = null)
        {
            DisplayName = displayName ?? string.Empty;
            PasswordHash = passwordHash;
            Username = username;
            Permissions = permissions;
            PictureUrl = pictureUrl ?? string.Empty;
        }

        /// <summary>
        /// Private constructor used to set the "IsAnyUser" flag
        /// </summary>
        /// <param name="isAnyUser"></param>
        /// <param name="username"></param>
        /// <param name="passwordHash"></param>
        /// <param name="displayName"></param>
        /// <param name="permissions"></param>
        private PatternzUser()
            : this(username: "AnyUser", passwordHash: string.Empty, permissions: Permission.None, displayName: string.Empty, pictureUrl: string.Empty)
        {
            IsAnyUser = true;
        }

        /// <summary>
        /// The display name of the current user
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// A hash of the user's password
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// What permissions the user has
        /// </summary>
        public Permission Permissions { get; set; }

        /// <summary>
        /// The username of the current user, used to login
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// A URL that points to the picture resource used by this user
        /// </summary>
        public string PictureUrl { get; set; }
    }
}
