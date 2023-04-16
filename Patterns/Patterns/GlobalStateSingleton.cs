namespace Patterns
{
    /// <summary>
    /// The gatekeeper for the global state
    /// </summary>
    public class GlobalStateSingleton
    {
        private static IGlobalState? _instance;

        /// <summary>
        /// Gets the instance of the global state
        /// </summary>
        public static IGlobalState Instance 
        { 
            get
            {
                return _instance ??= new GlobalState();
            }
        }
    }
}
