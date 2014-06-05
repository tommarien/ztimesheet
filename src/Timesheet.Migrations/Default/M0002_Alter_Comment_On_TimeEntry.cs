using FluentMigrator;

namespace Timesheet.Migrations.Default
{
    [Migration(20140605145657)]
    public class M0002_Alter_Comment_On_TimeEntry : ForwardOnlyMigration
    {
        public override void Up()
        {
            Alter.Table(M0001_Add_TimeEntry_Table.TableName)
                .AlterColumn("COMMENT").AsAnsiString(255).Nullable();
        }
    }
}