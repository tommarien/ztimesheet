using System;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices;
using Timesheet.ApplicationServices.Excel;

namespace Timesheet.Tests.Checksums
{
    [TestFixture]
    public class When_generating_checksum_for_a_bunch_of_time_entry_rows
    {
        [Test]
        public void it_is_instance_independent()
        {
            var entries = new[] {Create(), Create()};
            var otherEntries = new[] {Create(), Create()};

            entries.GenerateChecksum().ShouldBe(otherEntries.GenerateChecksum());
        }

        [Test]
        public void it_is_value_dependent()
        {
            var entries = new[] {Create(), Create()};
            var otherentries = new[] {Create()};

            entries.GenerateChecksum().ShouldNotBe(otherentries.GenerateChecksum());
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