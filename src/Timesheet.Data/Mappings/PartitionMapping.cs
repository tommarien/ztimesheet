using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;
using Timesheet.Domain.Entities;

namespace Timesheet.Data.Mappings
{
    public class PartitionMapping : ClassMapping<Partition>
    {
        public PartitionMapping()
        {
            Table("PARTITION");

            ComponentAsId(x => x.Key, m => m.Property(pk => pk.Value, cm =>
            {
                cm.Column("KEY");
                cm.Type(NHibernateUtil.AnsiString);
            }));

            Property(x => x.Checksum, m =>
            {
                m.Column("CHECKSUM");
                m.Type(NHibernateUtil.AnsiString);
            });

            Property(x => x.Revision, m => m.Column("REV"));
        }
    }
}