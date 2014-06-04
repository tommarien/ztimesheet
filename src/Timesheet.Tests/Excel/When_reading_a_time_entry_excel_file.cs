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
        private TimeEntryReader _timeEntryReader;
        private TimeEntryFilter _filter;
        private string _timeEntryExcelFile;

        [SetUp]
        public void Setup()
        {
            _timeEntryExcelFile = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Files", "Timesheet 2014 JD.xlsm"));

            _timeEntryReader = new TimeEntryReader();
            _filter = new TimeEntryFilter
            {
                Until = null,
                SkipEmptyLines = false
            };
        }

        [Test]
        public void it_skips_heading_lines()
        {
            var timeEntries = _timeEntryReader.Read(_timeEntryExcelFile, _filter);

            timeEntries.ShouldNotContain(timeEntry => timeEntry.Week == 0);
        }

        [Test]
        public void it_reads_up_until_requested_date()
        {
            _filter.Until = new DateTime(2014, 1, 4);

            var timeEntries = _timeEntryReader.Read(_timeEntryExcelFile, _filter);

            timeEntries.ShouldNotContain(timeEntry => timeEntry.Date >= _filter.Until);
        }

        [Test]
        public void it_skips_empty_lines_if_requested()
        {
            _filter.SkipEmptyLines = true;

            var timeEntries = _timeEntryReader.Read(_timeEntryExcelFile, _filter);

            timeEntries.ShouldNotContain(timeEntry => timeEntry.Hours.Equals(0) && timeEntry.ActivityCode == null);
        }

        [Test]
        public void it_does_not_skip_empty_lines_with_activity()
        {
            _filter.SkipEmptyLines = true;

            var timeEntries = _timeEntryReader.Read(_timeEntryExcelFile, _filter);

            timeEntries.ShouldContain(timeEntry => timeEntry.Date == new DateTime(2014, 1, 1));
        }

        [Test]
        public void it_reads_the_correct_data()
        {
            _filter.Until = new DateTime(2014, 1, 4);

            var timeEntries = _timeEntryReader.Read(_timeEntryExcelFile, _filter);

            timeEntries.Length.ShouldBe(3);

            EqualTimeEntry(timeEntries[0], new TimeEntry {UserInitials = "JD", ActivityCode = "HOLIDAY", Date = new DateTime(2014, 1, 1), Comment = "NEW YEAR"});
            EqualTimeEntry(timeEntries[1], new TimeEntry { UserInitials = "JD", ActivityCode = "DEV", Date = new DateTime(2014, 1, 2), Hours = 1.25, CustomerCode = "AMP", ProjectCode = "Flux Standard", WBSCode = "Server", Ticket = "AMP-0001", Comment = "Review tasks done by team" });
            EqualTimeEntry(timeEntries[2], new TimeEntry { UserInitials = "JD", ActivityCode = "DEV", Date = new DateTime(2014, 1, 3), Hours = 5, Kilometers = 20, CustomerCode = "FACQ", ProjectCode = "POD", WBSCode = "POD", Comment = "Review tasks done by team part II" });
        }

        private void EqualTimeEntry(TimeEntry actual, TimeEntry expected)
        {
            actual.UserInitials.ShouldBe(expected.UserInitials);
            actual.Date.ShouldBe(expected.Date);
            actual.ActivityCode.ShouldBe(expected.ActivityCode);
            actual.Hours.ShouldBe(expected.Hours);
            actual.Kilometers.ShouldBe(expected.Kilometers);
            actual.CustomerCode.ShouldBe(expected.CustomerCode);
            actual.ProjectCode.ShouldBe(expected.ProjectCode);
            actual.WBSCode.ShouldBe(expected.WBSCode);
            actual.Ticket.ShouldBe(expected.Ticket);
            actual.Comment.ShouldBe(expected.Comment);
        }
    }
}