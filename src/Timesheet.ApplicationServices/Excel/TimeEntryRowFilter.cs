using System;

namespace Timesheet.ApplicationServices.Excel
{
    public class TimeEntryRowFilter
    {
        public TimeEntryRowFilter()
        {
            Until = DateTime.Now.Date;
        }

        public DateTime? Until { get; set; }
        public bool SkipEmptyLines { get; set; }
    }
}