using Patterns.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        /// <param name="xmlFileName">The name of the DataFile to parse</param>
        /// <param name="dataFile">The resultant DataFile, if parsed correctly</param>
        /// <returns>True if the records were able to be parsed</returns>
        public bool TryParseRecords(string xmlFileName, out DataFile? dataFile)
        {
            dataFile = null;
            if (!File.Exists(xmlFileName))
            {
                return false;
            }

            try
            {
                XmlSerializer serializer = new(typeof(List<DataRecord>));
                List<DataRecord>? dataRecordsList = serializer.Deserialize(File.Open(xmlFileName, FileMode.Open)) as List<DataRecord>;

                dataFile = new DataFile()
                {
                    FileName = xmlFileName,
                    Format = DataRecordFormat.Xml,
                    Path = Path.GetDirectoryName(xmlFileName) ?? string.Empty
                };
                if (dataRecordsList != null)
                {
                    foreach (DataRecord record in dataRecordsList)
                    {
                        dataFile.DataRecords.Add(record.CreatedDate, record);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Writes a collection of data records to an XML file
        /// </summary>
        /// <param name="dataFile">The DataFile to write. It will be written to the FileName location</param>
        /// <returns>The number of bytes written</returns>
        public long WriteDataRecords(DataFile dataFile)
        {
            XmlSerializer serializer = new(typeof(List<DataRecord>));

            StreamWriter writer = new StreamWriter(dataFile.FileName);

            serializer.Serialize(writer, dataFile.DataRecords.Values.ToList());

            writer.Close();

            FileInfo fileJustWritten = new FileInfo(dataFile.FileName);
            return fileJustWritten.Length;
        }
    }
}
