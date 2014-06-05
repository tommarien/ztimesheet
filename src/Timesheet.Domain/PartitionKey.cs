using System;

namespace Timesheet.Domain
{
    /// <summary>
    /// Partition key wraps string in the form of yyyyMMdd|USER
    /// </summary>
    public class PartitionKey
    {
        public virtual string Value { get; protected set; }

        protected PartitionKey()
        {
        }

        public PartitionKey(DateTime dateTime, string user)
            : this()
        {
            Value = string.Format("{0:yyyyMMdd}|{1}", dateTime, user);
        }

        protected bool Equals(PartitionKey other)
        {
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PartitionKey) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}