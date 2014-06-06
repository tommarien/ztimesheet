using FluentMigrator;

namespace Timesheet.Migrations.Default
{
    [Migration(20140606160523)]
    public class M0005_Add_revision_to_Partition : Migration
    {
        public override void Up()
        {
            Alter.Table(M0003_Add_Partition_Table.TableName)
                .AddColumn("REV").AsInt32().NotNullable().WithDefaultValue(1);
        }

        public override void Down()
        {
            Delete.Column("REV").FromTable(M0003_Add_Partition_Table.TableName);
        }
    }
}