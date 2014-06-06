using System;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices;
using Timesheet.ApplicationServices.Excel;
using Timesheet.ApplicationServices.Processor;
using Timesheet.Domain;
using Timesheet.Domain.Entities;

namespace Timesheet.Tests.Processor
{
    [TestFixture]
    public class When_processing_modified_time_entries : IntegrationTestBase
    {
        protected override void AfterTestFixtureSetup()
        {
            ExcelRecords = new[]
            {
                new TimeEntryRow
                {
                    Date = new DateTime(2001, 1, 1),
                    Activity = "DEV",
                    Customer = "XXX",
                    Hours = 8,
                    Kilometers = 0,
                    Project = "PROX",
                    User = "JD",
                    WBSCode = "POD",
                    Ticket = "ABC-001",
                    Comment = "A comment"
                },
                new TimeEntryRow
                {
                    Date = new DateTime(2001, 1, 2),
                    Activity = "DEV",
                    Customer = "XXX",
                    Hours = 2.5,
                    Kilometers = 15,
                    Project = "PROX",
                    User = "JD",
                    WBSCode = "POD",
                    Ticket = "ABC-001",
                    Comment = "A comment"
                },
                new TimeEntryRow
                {
                    Date = new DateTime(2001, 1, 2),
                    Activity = "DEV",
                    Customer = "YYY",
                    Hours = 2.5,
                    Kilometers = 15,
                    Project = "PROX",
                    User = "AB",
                    WBSCode = "POD",
                    Ticket = "ABC-001",
                    Comment = "Another comment"
                }
            };

            using (var session = SessionFactory.OpenStatelessSession())
            {
                var row1 = new TimeEntryRow
                {
                    Date = new DateTime(2001, 1, 1),
                    Activity = "DEV",
                    Customer = "XXX",
                    Hours = 4,
                    Kilometers = 0,
                    Project = "PROX",
                    User = "JD",
                    WBSCode = "POD",
                    Ticket = "ABC-001",
                    Comment = "A comment"
                };
                var row2 = new TimeEntryRow
                {
                    Date = new DateTime(2001, 1, 1),
                    Activity = "DEV",
                    Customer = "XXX",
                    Hours = 4,
                    Kilometers = 0,
                    Project = "PROX",
                    User = "JD",
                    WBSCode = "POD",
                    Ticket = "ABC-001",
                    Comment = "A comment"
                };
                ExistingPartitionKey = (PartitionKey)session.Insert(new Partition
                {
                    Key = new PartitionKey(row1.Date, row1.User),
                    Checksum = new[] {row1, row2}.GenerateChecksum()
                });

                var entry1 = (Guid)session.Insert(row1.CreateTimeEntry());
                var entry2 = (Guid)session.Insert(row2.CreateTimeEntry());
                ExistingTimeEntryIds = new[] {entry1, entry2};
            }
        }

        public TimeEntryRow[] ExcelRecords { get; private set; }
        public PartitionKey ExistingPartitionKey { get; private set; }
        public Guid[] ExistingTimeEntryIds { get; private set; }

        private void Act()
        {
            var processor = new TimeEntryProcessor(SessionFactory);
            processor.Process(ExcelRecords);
        }

        [Test]
        public void it_should_add_partition_keys_to_the_db()
        {
            Act();

            using (var session = SessionFactory.OpenStatelessSession())
            {
                session.Query<Partition>().Count().ShouldBe(3);
            }
        }

        [Test]
        public void it_should_add_time_entries_to_the_db()
        {
            Act();

            using (var session = SessionFactory.OpenStatelessSession())
            {
                session.Query<TimeEntry>().Count().ShouldBe(ExcelRecords.Length);
            }
        }

        [Test]
        public void it_should_remove_existing_time_entries_impacted_by_changes()
        {
            Act();

            using (var session = SessionFactory.OpenStatelessSession())
            {
                session.Query<TimeEntry>().Count(x => ExistingTimeEntryIds.Contains(x.Id)).ShouldBe(0);
            }
        }

        [Test]
        public void it_should_add_time_entries_with_correct_data()
        {
            Act();

            using (var session = SessionFactory.OpenStatelessSession())
            {
                var entries = session.Query<TimeEntry>().ToList();

                var entry1 = entries.FirstOrDefault(e => e.Date == new DateTime(2001, 1, 1) && e.User == "JD");
                var entry2 = entries.FirstOrDefault(e => e.Date == new DateTime(2001, 1, 2) && e.User == "JD");
                var entry3 = entries.FirstOrDefault(e => e.Date == new DateTime(2001, 1, 2) && e.User == "AB");

                EqualTimeEntry(entry1, ExcelRecords[0]);
                EqualTimeEntry(entry2, ExcelRecords[1]);
                EqualTimeEntry(entry3, ExcelRecords[2]);
            }
        }

        [Test]
        public void it_should_add_new_partition_keys_with_correct_data()
        {
            Act();

            using (var session = SessionFactory.OpenStatelessSession())
            {
                var partitionKeys = session.Query<Partition>();

                var key2 = partitionKeys.FirstOrDefault(k => k.Key.Value == "20010102|JD");
                var key3 = partitionKeys.FirstOrDefault(k => k.Key.Value == "20010102|AB");

                key2.Checksum.ShouldBe(ExcelRecords.Where(x => x.Date == new DateTime(2001, 1, 2) && x.User == "JD").GenerateChecksum());
                key3.Checksum.ShouldBe(ExcelRecords.Where(x => x.Date == new DateTime(2001, 1, 2) && x.User == "AB").GenerateChecksum());
            }
        }

        [Test]
        public void it_should_update_existing_partition_key_checksum()
        {
            Act();

            using (var session = SessionFactory.OpenStatelessSession())
            {
                var existingPartition = session.Get<Partition>(ExistingPartitionKey);
                existingPartition.Checksum.ShouldBe(ExcelRecords.Where(x => x.Date == new DateTime(2001, 1, 1) && x.User == "JD").GenerateChecksum());
            }
        }

        private void EqualTimeEntry(TimeEntry actual, TimeEntryRow expected)
        {
            actual.User.ShouldBe(expected.User);
            actual.Date.ShouldBe(expected.Date);
            actual.Activity.ShouldBe(expected.Activity);
            actual.Hours.ShouldBe(expected.Hours);
            actual.Kilometers.ShouldBe(expected.Kilometers);
            actual.Customer.ShouldBe(expected.Customer);
            actual.Project.ShouldBe(expected.Project);
            actual.WBSCode.ShouldBe(expected.WBSCode);
            actual.Ticket.ShouldBe(expected.Ticket);
            actual.Comment.ShouldBe(expected.Comment);
        }
    }
}