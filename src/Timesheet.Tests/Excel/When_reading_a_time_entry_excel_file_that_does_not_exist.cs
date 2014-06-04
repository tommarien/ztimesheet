using System.IO;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices.Excel;

namespace Timesheet.Tests.Excel
{
    [TestFixture]
    public class When_reading_a_time_entry_excel_file_that_does_not_exist
    {
        private TimeEntryReader _timeEntryReader;
        private TimeEntryFilter _filter;

        [SetUp]
        public void Setup()
        {
            _timeEntryReader = new TimeEntryReader();
            _filter = new TimeEntryFilter();
        }

        [Test]
        public void it_throws_a_file_not_found_exception()
        {
            var file = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Files", "Unexisting.xlsm"));

            Should.Throw<FileNotFoundException>(() => _timeEntryReader.Read(file, _filter));
        }
    }
}