using System.IO;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices.Excel;

namespace Timesheet.Tests.Excel
{
    [TestFixture]
    public class When_reading_a_time_entry_excel_file_that_does_not_exist
    {
        private TimeEntryRowReader _timeEntryRowReader;
        private TimeEntryRowFilter _rowFilter;

        [SetUp]
        public void Setup()
        {
            _timeEntryRowReader = new TimeEntryRowReader();
            _rowFilter = new TimeEntryRowFilter();
        }

        [Test]
        public void it_throws_a_file_not_found_exception()
        {
            var file = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Files", "Unexisting.xlsm"));

            Should.Throw<FileNotFoundException>(() => _timeEntryRowReader.Read(file, _rowFilter));
        }
    }
}