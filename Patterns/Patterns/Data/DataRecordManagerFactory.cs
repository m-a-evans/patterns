using Patterns.Account.Model;
using Patterns.Data;
using Patterns.Data.Model;

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
        /// <param name="format">The format of DataRecordManager to get</param>
        /// <returns>The concrete instance</returns>
        public static IDataRecordManager GetUserRecordManager(IPatternzUser currentUser, DataRecordFormat format)
        {
            switch (format)
            {
                case DataRecordFormat.Xml:
                    return new IOProxy(currentUser, new XmlDataRecordManager());
                case DataRecordFormat.Json:
                default:
                    return new IOProxy(currentUser, new JsonDataRecordManager());
            }
        }
    }
}
