using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Patterns.Data.Model
{
    /// <summary>
    /// A record of data
    /// </summary>
    public class DataRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private static readonly DataRecord _emptyRecord = new DataRecord()
        {
            Id = Guid.Empty,
            DateModified = DateTime.MinValue,
            DataRecordName = string.Empty,
            Description = string.Empty,
            CreatedDate = DateTime.MinValue
        };
        public static DataRecord Empty => _emptyRecord;

        private Guid _id = Guid.Empty;
        private DateTime _dateModified = DateTime.MinValue;
        private string _description = string.Empty;
        private string _dataRecordName = string.Empty;
        private DateTime _createdDate = DateTime.MinValue;

        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    OnPropertyChanging();
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }
        public string DataRecordName 
        { 
            get
            {
                return _dataRecordName;
            }
            set
            {
                if (_dataRecordName != value) 
                {
                    OnPropertyChanging();
                    _dataRecordName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    OnPropertyChanging();
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime DateModified
        {
            get
            {
                return _dateModified;
            }
            set
            {
                if (_dateModified != value)
                {
                    OnPropertyChanging();
                    _dateModified = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime CreatedDate
        {
            get
            {
                return _createdDate;
            }
            set
            {
                if (_createdDate != value)
                {
                    OnPropertyChanging();
                    _createdDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangingEventHandler? PropertyChanging;

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

        /// <summary>
        /// For simplicity, a data record is considered equal only if the GUID is equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DataRecord other) 
        {
            if (ReferenceEquals(null, other)) 
                return false;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        protected void OnPropertyChanging([CallerMemberName] string? propName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propName));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
