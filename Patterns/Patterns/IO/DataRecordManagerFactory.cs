using Patterns.Model.Data;

namespace Patterns.IO
{
    /// <summary>
    /// Factory for creating a DataRecordManager
    /// </summary>
    public static class DataRecordManagerFactory
    {
        /// <summary>
        /// Gets a concrete instance of a DataRecordManager based on the provided type
        /// </summary>
        /// <param name="type">The type of DataRecordManager to get</param>
        /// <returns>The concrete instance</returns>
        public static IDataRecordManager GetUserRecordManager(DataRecordManagerType type)
        {
            switch (type)
            {
                case DataRecordManagerType.Xml:
                    return new XmlDataRecordManager();
                case DataRecordManagerType.Json:
                default:
                    return new JsonDataRecordManager();
            }
        }
    }
}
