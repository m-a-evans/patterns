using System;

namespace Patterns.Data.Model
{
    /// <summary>
    /// A record of data
    /// </summary>
    public class DataRecord
    {
        public Guid Id { get; set; }
        public string DataRecordName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;

        public DateTime DateModified { get; set; }

        public DataRecord DeepCopy()
        {
            return new DataRecord
            {
                Id = Id,
                DataRecordName = DataRecordName,
                Description = Description,
                CreatedDate = CreatedDate,
                DateModified = DateModified
            };
        }
    }
}
