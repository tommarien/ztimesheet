using FluentMigrator;

namespace Timesheet.Migrations.Default
{
    [Migration(20140605152800)]
    public class M0003_Add_Partition_Table : AutoReversingMigration
    {
        public const string TableName = "PARTITION";
        public const string PrimaryKey = "KEY";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn(PrimaryKey).AsAnsiString(20).NotNullable().PrimaryKey()
                .WithColumn("CHECKSUM").AsAnsiString(40).NotNullable();
        }
    }
}