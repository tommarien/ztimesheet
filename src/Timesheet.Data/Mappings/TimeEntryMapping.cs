using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Timesheet.Domain.Entities;

namespace Timesheet.Data.Mappings
{
    public class TimeEntryMapping : ClassMapping<TimeEntry>
    {
        public TimeEntryMapping()
        {
            Table("TIME_ENTRY");

            Id(x => x.Id, m =>
            {
                m.Column("TIME_ENTRY_ID");
                m.Generator(Generators.GuidComb);
            });

            Property(x => x.User, m =>
            {
                m.Column("USER");
                m.Type(NHibernateUtil.AnsiString);
            });

            Property(x => x.Date, m =>
            {
                m.Column("DATE");
                m.Type(NHibernateUtil.Date);
            });

            Property(x => x.Activity, m =>
            {
                m.Column("ACTIVITY");
                m.Type(NHibernateUtil.AnsiString);
            });

            Property(x => x.Hours, m => m.Column("HOURS"));
            Property(x => x.Kilometers, m => m.Column("KM"));

            Property(x => x.Customer, m =>
            {
                m.Column("CUSTOMER");
                m.Type(NHibernateUtil.AnsiString);
            });

            Property(x => x.Project, m =>
            {
                m.Column("PROJECT");
                m.Type(NHibernateUtil.AnsiString);
            });

            Property(x => x.WBSCode, m =>
            {
                m.Column("WBS_CODE");
                m.Type(NHibernateUtil.AnsiString);
            });

            Property(x => x.Ticket, m =>
            {
                m.Column("TICKET");
                m.Type(NHibernateUtil.AnsiString);
            });

            Property(x => x.Comment, m =>
            {
                m.Column(cm =>
                {
                    cm.Name("COMMENT");
                    cm.SqlType("varchar(max)");
                });
                m.Type(NHibernateUtil.StringClob);
            });
        }
    }
} ;