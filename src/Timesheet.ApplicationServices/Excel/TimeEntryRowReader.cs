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

            using (var excelQueryFactory = new ExcelQueryFactory(fileName))
            {
                IQueryable<TimeEntryRow> query = excelQueryFactory.Worksheet<TimeEntryRow>("Data")
                    .Where(x => x.Date != DateTime.MinValue);

                if (rowFilter.Until.HasValue)
                    query = query.Where(x => x.Date < rowFilter.Until.Value);

                if (rowFilter.SkipEmptyLines)
                    query = query.Where(x => x.Hours > 0 || x.Activity != null);

                return query.ToArray();
            }
        }
    }
}