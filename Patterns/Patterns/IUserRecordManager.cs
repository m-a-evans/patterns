using Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    public interface IUserRecordManager
    {
        public long WriteUserRecords(string collectionName, List<UserRecord> userRecord);

        public bool TryParseRecords(string recordName, out List<UserRecord> userRecord);
    }
}
