using System.IO;
using System.Linq;
using LinqToExcel;

namespace Timesheet.ApplicationServices.Excel
{
    public class TimeEntryReader
    {
        public TimeEntry[] Read(string fileName, TimeEntryFilter filter)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("TimeEntry file cannot be found", fileName);

            using (var excelQueryFactory = new ExcelQueryFactory(fileName))
            {
                IQueryable<TimeEntry> query = excelQueryFactory.Worksheet<TimeEntry>("Data")
                    .Where(x => x.Week > 0);

                if (filter.Until.HasValue)
                    query = query.Where(x => x.Date < filter.Until.Value);

                if (filter.SkipEmptyLines)
                    query = query.Where(x => x.Hours > 0 || x.ActivityCode != null);

                return query.ToArray();
            }
        }
    }
}