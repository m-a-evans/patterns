namespace Patterns.Account.Model
{
    /// <summary>
    /// Interface representing a user of the Patternz app
    /// </summary>
    public interface IPatternzUser
    {
        /// <summary>
        /// A flag indicating "any old user"
        /// </summary>
        bool IsAnyUser { get; }

        /// <summary>
        /// The display name of the current user
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// A hash of the user's password
        /// </summary>
        string PasswordHash { get; set; }

        /// <summary>
        /// What permissions the user has
        /// </summary>
        Permission Permissions { get; set; }

        /// <summary>
        /// The username of the current user, used to login
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// A URL that points to the picture resource used by this user
        /// </summary>
        string PictureUrl { get; set; }
    }
}
