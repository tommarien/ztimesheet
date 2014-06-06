using System;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices.Excel;
using Timesheet.ApplicationServices.Processor;
using Timesheet.Domain.Entities;

namespace Timesheet.Tests.Processor
{
    [TestFixture]
    public class When_processing_time_entries_with_an_empty_db : IntegrationTestBase
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
                    Hours = 4,
                    Kilometers = 0,
                    Project = "PROX",
                    User = "JD",
                    WBSCode = "POD",
                    Ticket = "ABC-001",
                    Comment = "A comment"
                },
                new TimeEntryRow
                {
                    Date = new DateTime(2001, 1, 1),
                    Activity = "ANA",
                    Customer = "XXX",
                    Hours = 4,
                    Kilometers = 10,
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
        }

        public TimeEntryRow[] ExcelRecords { get; private set; }

        private void Act()
        {
            var processor = new TimeEntryProcessor(SessionFactory);
            processor.Process(ExcelRecords);
        }

        [Test]
        public void it_should_add_partition_key_to_the_db()
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
    }
}