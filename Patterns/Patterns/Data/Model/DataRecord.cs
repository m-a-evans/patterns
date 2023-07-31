using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Chat;

namespace Patterns.Data.Model
{
    /// <summary>
    /// A record of data
    /// </summary>
    public class DataRecord : INotifyPropertyChanged, INotifyPropertyChanging
    {
        #region Fields and Constants
        private static readonly DataRecord _emptyRecord = new DataRecord()
        {
            Id = Guid.Empty,
            DateModified = DateTime.MinValue,
            DataRecordName = string.Empty,
            Description = string.Empty,
            CreatedDate = DateTime.MinValue
        };

        private Guid _id = Guid.Empty;
        private DateTime _dateModified = DateTime.MinValue;
        private string _description = string.Empty;
        private string _dataRecordName = string.Empty;
        private DateTime _createdDate = DateTime.MinValue;
        #endregion

        #region Properties and Constructors
        public static DataRecord Empty => _emptyRecord;
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

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new DataRecord identical to this one. Does not
        /// copy event handlers.
        /// </summary>
        /// <returns></returns>
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

        public override string ToString()
        {
            return $"DataRecord: [ Id: {Id},\nName: \"{DataRecordName}\"\nDescription: \"{Description}\" ]\n";
        }

        /// <summary>
        /// For simplicity, a data record is considered equal only if the GUID is equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DataRecord? other) 
        {
            if (ReferenceEquals(null, other)) 
                return false;
            return other.Id == Id;
        }

        /// <summary>
        /// Returns a hash code based on the object's Id
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion

        #region Private Methods
        protected void OnPropertyChanging([CallerMemberName] string? propName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propName));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        #region Operators
        public static bool operator ==(DataRecord? a, DataRecord? b)
        {
            if (!ReferenceEquals(null, a))
            {
                return a.Equals(b);
            }
            if (!ReferenceEquals(null, b))
            {
                return b.Equals(a);
            }
            return true;
        }

        public static bool operator !=(DataRecord? a, DataRecord? b)
        {
            return !(a == b);
        }
        #endregion
    }
}
