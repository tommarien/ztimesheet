namespace Timesheet.Domain.Entities
{
    public class Partition
    {
        public Partition()
        {
            Revision = 1;
        }

        public virtual PartitionKey Key { get; set; }
        public virtual string Checksum { get; set; }
        public virtual int Revision { get; set; }
    }
}