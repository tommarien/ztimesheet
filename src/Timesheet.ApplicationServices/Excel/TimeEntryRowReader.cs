using System;
using System.IO;
using System.Linq;
using LinqToExcel;

namespace Timesheet.ApplicationServices.Excel
{
    public class TimeEntryRowReader
    {
        public TimeEntryRow[] Read(string fileName, TimeEntryRowFilter rowFilter)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("TimeEntry file cannot be found", fileName);

            TimeEntryRow[] entries;

            using (var excelQueryFactory = new ExcelQueryFactory(fileName))
            {
                IQueryable<TimeEntryRow> query = excelQueryFactory.Worksheet<TimeEntryRow>("Data")
                    .Where(x => x.Date != DateTime.MinValue);

                if (rowFilter.Until.HasValue)
                    query = query.Where(x => x.Date < rowFilter.Until.Value);

                entries = query.ToArray();
            }

            // This is to handle a weird case that happens in LinqToExcel, apparantly it does not
            // work perfect
            if (rowFilter.SkipEmptyLines)
                entries = entries.Where(x => x.Hours > 0 || x.Activity != null).ToArray();

            return entries;
        }
    }
}