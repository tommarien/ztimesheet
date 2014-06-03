using System.Linq;
using LinqToExcel;

namespace Timesheet.ApplicationServices.Excel
{
    public class TimeEntryReader
    {
        public TimeEntry[] ReadAll(string fileName, TimeEntryFilter filter)
        {
            using (var excelQueryFactory = new ExcelQueryFactory(fileName))
            {
                IQueryable<TimeEntry> query = excelQueryFactory.Worksheet<TimeEntry>("Data")
                    .Where(x => x.Week > 0);

                if (filter.Until.HasValue)
                    query = query.Where(x => x.Date < filter.Until.Value);

                if (filter.SkipEmptyLines)
                    query = query.Where(x => x.Hours > 0);

                return query.ToArray();
            }
        }
    }
}