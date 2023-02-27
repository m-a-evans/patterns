using Patterns.IO;
using Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    public static class UserRecordManagerFactory
    {
        public static IUserRecordManager GetUserRecordManager(UserRecordManagerType type)
        {
            switch (type)
            {
                case UserRecordManagerType.Xml:
                    return new XmlUserRecordManager();
                case UserRecordManagerType.Json:
                default:
                    return new JsonUserRecordManager();
            }
        }
    }
}
