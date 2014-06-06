using System;
using System.IO;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices.Excel;

namespace Timesheet.Tests.Excel
{
    [TestFixture]
    public class When_reading_a_time_entry_excel_file
    {
        private TimeEntryRowReader _timeEntryRowReader;
        private TimeEntryRowFilter _rowFilter;
        private string _timeEntryExcelFile;

        [SetUp]
        public void Setup()
        {
            _timeEntryExcelFile = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Files", "Timesheet 2014 JD.xlsm"));

            _timeEntryRowReader = new TimeEntryRowReader();
            _rowFilter = new TimeEntryRowFilter
            {
                Until = null,
                SkipEmptyLines = false
            };
        }

        [Test]
        public void it_skips_heading_lines()
        {
            var timeEntries = _timeEntryRowReader.Read(_timeEntryExcelFile, _rowFilter);

            timeEntries.ShouldNotContain(timeEntry => timeEntry.Date == DateTime.MinValue);
        }

        [Test]
        public void it_skips_heading_lines_even_if_we_specify_until()
        {
            _rowFilter.Until = new DateTime(2014, 1, 4);

            var timeEntries = _timeEntryRowReader.Read(_timeEntryExcelFile, _rowFilter);

            timeEntries.ShouldNotContain(timeEntry => timeEntry.Date == DateTime.MinValue);
        }

        [Test]
        public void it_skips_heading_lines_even_if_we_specify_skip_empty_lines()
        {
            _rowFilter.SkipEmptyLines = true;

            var timeEntries = _timeEntryRowReader.Read(_timeEntryExcelFile, _rowFilter);

            timeEntries.ShouldNotContain(timeEntry => timeEntry.Date == DateTime.MinValue);
        }

        [Test]
        public void it_reads_up_until_requested_date()
        {
            _rowFilter.Until = new DateTime(2014, 1, 4);

            var timeEntries = _timeEntryRowReader.Read(_timeEntryExcelFile, _rowFilter);

            timeEntries.ShouldNotContain(timeEntry => timeEntry.Date >= _rowFilter.Until);
        }

        [Test]
        public void it_skips_empty_lines_if_requested()
        {
            _rowFilter.SkipEmptyLines = true;

            var timeEntries = _timeEntryRowReader.Read(_timeEntryExcelFile, _rowFilter);

            timeEntries.ShouldNotContain(timeEntry => timeEntry.Hours.Equals(0) && timeEntry.Activity == null);
        }

        [Test]
        public void it_does_not_skip_empty_lines_with_activity()
        {
            _rowFilter.SkipEmptyLines = true;

            var timeEntries = _timeEntryRowReader.Read(_timeEntryExcelFile, _rowFilter);

            timeEntries.ShouldContain(timeEntry => timeEntry.Date == new DateTime(2014, 1, 1));
        }

        [Test]
        public void it_reads_the_correct_data()
        {
            _rowFilter.Until = new DateTime(2014, 1, 4);

            var timeEntries = _timeEntryRowReader.Read(_timeEntryExcelFile, _rowFilter);

            timeEntries.Length.ShouldBe(3);

            EqualTimeEntry(timeEntries[0], new TimeEntryRow {User = "JD", Activity = "HOLIDAY", Date = new DateTime(2014, 1, 1), Comment = "NEW YEAR"});
            EqualTimeEntry(timeEntries[1], new TimeEntryRow { User = "JD", Activity = "DEV", Date = new DateTime(2014, 1, 2), Hours = 1.25, Customer = "AMP", Project = "Flux Standard", WBSCode = "Server", Ticket = "AMP-0001", Comment = "Review tasks done by team" });
            EqualTimeEntry(timeEntries[2], new TimeEntryRow { User = "JD", Activity = "DEV", Date = new DateTime(2014, 1, 3), Hours = 5, Kilometers = 20, Customer = "FACQ", Project = "POD", WBSCode = "POD", Comment = "Review tasks done by team part II" });
        }

        private void EqualTimeEntry(TimeEntryRow actual, TimeEntryRow expected)
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