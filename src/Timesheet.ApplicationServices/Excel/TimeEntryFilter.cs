using System;

namespace Timesheet.ApplicationServices.Excel
{
    public class TimeEntryFilter
    {
        public TimeEntryFilter()
        {
            Until = DateTime.Now.Date;
        }

        public DateTime? Until { get; set; }
        public bool SkipEmptyLines { get; set; }
    }
}