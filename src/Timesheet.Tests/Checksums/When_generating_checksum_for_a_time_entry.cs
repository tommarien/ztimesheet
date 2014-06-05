using System;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices;
using Timesheet.ApplicationServices.Excel;

namespace Timesheet.Tests.Checksums
{
    [TestFixture]
    public class When_generating_checksum_for_a_time_entry
    {
        private TimeEntryRow[] rows;

        [SetUp]
        public void Setup()
        {
            rows = new[] {Create()};
        }

        [Test]
        public void it_generates_the_same_checksum_if_data_is_equal()
        {
            rows.GenerateChecksum().ShouldBe(rows.GenerateChecksum());
        }

        [Test]
        public void it_generates_different_checksum_if_data_is_different()
        {
            var originalChecksum = rows.GenerateChecksum();

            rows[0].Activity = "DEV1";

            rows.GenerateChecksum().ShouldNotBe(originalChecksum);
        }

        [Test]
        public void it_generates_different_checksum_if_we_swap_data()
        {
            var originalChecksum = rows.GenerateChecksum();

            rows[0].Activity = "XXX";
            rows[0].Customer = "DEV";

            rows.GenerateChecksum().ShouldNotBe(originalChecksum);
        }

        private TimeEntryRow Create()
        {
            var row = new TimeEntryRow
            {
                Date = new DateTime(2001, 1, 1),
                Activity = "DEV",
                Customer = "XXX",
                Hours = 2.5,
                Kilometers = 15,
                Project = "PROX",
                User = "JD",
                WBSCode = "POD",
                Ticket = "ABC-001",
                Comment = "A comment"
            };

            return row;
        }
    }
}