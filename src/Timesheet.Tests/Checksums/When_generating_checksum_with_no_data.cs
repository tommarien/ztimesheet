using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices;
using Timesheet.ApplicationServices.Excel;

namespace Timesheet.Tests.Checksums
{
    [TestFixture]
    public class When_generating_checksum_with_no_data
    {
        [Test]
        public void it_returns_null_as_checksum()
        {
            TimeEntryRow[] rows = new TimeEntryRow[0];

            var checksum = rows.GenerateChecksum();

            checksum.ShouldBe(null);
        }
    }
}