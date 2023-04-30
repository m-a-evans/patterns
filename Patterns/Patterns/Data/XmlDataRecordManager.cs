using Patterns.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Patterns.IO
{
    /// <summary>
    /// Implementation of the data record manager interface that handles XML files
    /// </summary>
    internal class XmlDataRecordManager : IDataRecordManager
    {
        /// <summary>
        /// Attempts to parse a collection of data records by xml file name
        /// </summary>
        /// <param name="xmlFileName">The name of the data records to parse</param>
        /// <param name="dataRecords">The resultant list of records, if parsed successfully</param>
        /// <returns>True if the records were able to be parsed</returns>
        public bool TryParseRecords(string xmlFileName, out List<DataRecord>? userRecords)
        {
            userRecords = new List<DataRecord>();
            if (!File.Exists(xmlFileName))
            {
                return false;
            }

            try
            {
                XmlSerializer serializer = new(typeof(List<DataRecord>));
                userRecords = serializer.Deserialize(File.Open(xmlFileName, FileMode.Open)) as List<DataRecord>;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Writes a collection of data records to an XML file
        /// </summary>
        /// <param name="collectionName">The name of the collection to write</param>
        /// <param name="dataRecords">The collection to write</param>
        /// <returns>The number of bytes written</returns>
        public long WriteDataRecords(string collectionName, List<DataRecord> userRecords)
        {
            XmlSerializer serializer = new(typeof(List<DataRecord>));
            StreamWriter writer = new StreamWriter(collectionName);

            serializer.Serialize(writer, userRecords);

            writer.Close();

            FileInfo fileJustWritten = new FileInfo(collectionName);
            return fileJustWritten.Length;
        }
    }
}
