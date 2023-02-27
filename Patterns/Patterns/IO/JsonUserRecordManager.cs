using Patterns.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Patterns.IO
{
    internal class JsonUserRecordManager : IUserRecordManager
    {
        public bool TryParseRecords(string recordName, out List<UserRecord> userRecords)
        {
            userRecords = new List<UserRecord>();
            if (!File.Exists(recordName))
            {
                return false;
            }

            UserRecord[]? userRecordsAsArr = JsonSerializer.Deserialize<UserRecord[]>(File.ReadAllText(recordName));

            if (userRecordsAsArr == null)
            {
                return false;
            }
            else
            {
                userRecords = new List<UserRecord>(userRecordsAsArr);
                return true;
            }
        }

        public long WriteUserRecords(string collectionName, List<UserRecord> userRecords)
        {
            UserRecord[] userRecordsAsArr = userRecords.ToArray();

            string serialized = JsonSerializer.Serialize(userRecordsAsArr);
            File.WriteAllText(collectionName, serialized);

            return Encoding.UTF8.GetByteCount(serialized);
        }
    }
}
