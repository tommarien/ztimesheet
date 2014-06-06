using FluentMigrator;

namespace Timesheet.Migrations.Default
{
    [Migration(20140606164054)]
    public class M0006_Add_CreatedOn_To_TimeEntry : Migration
    {
        public override void Up()
        {
            Create.Column("CREATED_ON").OnTable(M0001_Add_TimeEntry_Table.TableName)
                .AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            Delete.Column("CREATED_ON").FromTable(M0001_Add_TimeEntry_Table.TableName);
        }
    }
}