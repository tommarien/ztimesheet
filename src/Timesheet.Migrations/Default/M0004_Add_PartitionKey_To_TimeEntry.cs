using FluentMigrator;

namespace Timesheet.Migrations.Default
{
    [Migration(20140605154300)]
    public class M0004_Add_PartitionKey_To_TimeEntry : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table(M0001_Add_TimeEntry_Table.TableName)
                .AddColumn("PARTITION_KEY").AsAnsiString(20).NotNullable().Indexed();
        }
    }
}