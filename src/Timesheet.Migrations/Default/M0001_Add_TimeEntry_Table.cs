using FluentMigrator;

namespace Timesheet.Migrations.Default
{
    [Migration(20140605135647)]
    public class M0001_Add_TimeEntry_Table : AutoReversingMigration
    {
        public const string TableName = "TIME_ENTRY";
        public const string PrimaryKey = "TIME_ENTRY_ID";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn(PrimaryKey).AsGuid().WithDefault(SystemMethods.NewGuid).NotNullable().PrimaryKey()
                .WithColumn("USER").AsAnsiString(10).NotNullable()
                .WithColumn("DATE").AsDate().NotNullable()
                .WithColumn("ACTIVITY").AsAnsiString(20).Nullable()
                .WithColumn("HOURS").AsDouble().NotNullable().WithDefaultValue(0)
                .WithColumn("KM").AsDouble().NotNullable().WithDefaultValue(0)
                .WithColumn("CUSTOMER").AsAnsiString(80).Nullable()
                .WithColumn("PROJECT").AsAnsiString(100).Nullable()
                .WithColumn("WBS_CODE").AsAnsiString(80).Nullable()
                .WithColumn("TICKET").AsAnsiString(80).Nullable()
                .WithColumn("COMMENT").AsAnsiString(int.MaxValue).Nullable();
        }
    }
}