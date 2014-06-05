using System;
using LinqToExcel.Attributes;

namespace Timesheet.ApplicationServices.Excel
{
    public class TimeEntryRow
    {
        [ExcelColumn("Name")]
        public string User { get; set; }

        public DateTime Date { get; set; }

        [ExcelColumn("Type")]
        public string Activity { get; set; }

        public double Hours { get; set; }

        [ExcelColumn("Km")]
        public double Kilometers { get; set; }

        public string Customer { get; set; }
        public string Project { get; set; }

        [ExcelColumn("WBS code")]
        public string WBSCode { get; set; }

        public string Ticket { get; set; }
        public string Comment { get; set; }
    }
}