using Patterns.Account.Model;

namespace PatternsUI.Model
{
    /// <summary>
    /// Represents a user's individual permission in a display friendly way
    /// </summary>
    public class UserPermission
    {
        public string Name { get => GetName(); }
        public Permission Permission { get; private set; }
        public bool IsEnabled { get; set; }

        public UserPermission(Permission permission)
        {
            Permission = permission;
        }

        /// <summary>
        /// Gets the display friendly name
        /// </summary>
        /// <returns></returns>
        private string GetName()
        {
            switch (Permission)
            {
                case Permission.AddUser:
                    return "Add Users";
                case Permission.ReadAccess:
                    return "Read Data Records";
                case Permission.WriteAccess:
                    return "Write Data Records";
                case Permission.UpdateUser:
                    return "Update Users";
                case Permission.RemoveUser:
                    return "Remove Users";
                default:
                    return string.Empty;
            }
        }
    }
}
