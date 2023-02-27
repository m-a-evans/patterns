using Patterns.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Patterns.IO
{
    internal class XmlUserRecordManager : IUserRecordManager
    {
        public bool TryParseRecords(string recordName, out List<UserRecord> userRecords)
        {
            userRecords = new List<UserRecord>();
            if (!File.Exists(recordName))
            {
                return false;
            }

            try
            {
                XmlSerializer serializer = new(typeof(List<UserRecord>));
                userRecords = serializer.Deserialize(File.Open(recordName, FileMode.Open)) as List<UserRecord>;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public long WriteUserRecords(string collectionName, List<UserRecord> userRecords)
        {
            XmlSerializer serializer = new(typeof(List<UserRecord>));
            StreamWriter writer = new StreamWriter(collectionName);

            serializer.Serialize(writer, userRecords);

            writer.Close();

            FileInfo fileJustWritten = new FileInfo(collectionName);
            return fileJustWritten.Length;
        }
    }
}
