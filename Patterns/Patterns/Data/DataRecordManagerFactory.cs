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
        const string Xml = "xml";
        const string Json = "json";

        /// <summary>
        /// Gets a concrete instance of a DataRecordManager based on the provided type
        /// </summary>
        /// <param name="currentUser">The user trying to use the DataRecordManager</param>
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

        /// <summary>
        /// Gets a concrete instance of a DataRecordManager based on the extension of the file name
        /// </summary>
        /// <param name="currentUser">The user trying to use the DataRecordManager</param>
        /// <param name="fileName">The filename including extension the user intends to parse</param>
        /// <returns>The concrete instance</returns>
        public static IDataRecordManager GetUserRecordManager(IPatternzUser currentUser, string fileName)
        {
            if (fileName.ToLower().EndsWith(Xml))
            {
                return new IOProxy(currentUser, new XmlDataRecordManager());
            }
            else
            {
                return new IOProxy(currentUser, new JsonDataRecordManager());
            }
        }
    }
}
