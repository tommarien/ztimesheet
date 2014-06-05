namespace Timesheet.Domain.Entities
{
    public class Partition
    {
        public virtual PartitionKey Key { get; set; }
        public virtual string CheckSum { get; set; }
    }
}