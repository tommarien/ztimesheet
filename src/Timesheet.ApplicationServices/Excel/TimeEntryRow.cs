using System;
using LinqToExcel.Attributes;

namespace Timesheet.ApplicationServices.Excel
{
    public class TimeEntryRow
    {
        [ExcelColumn("Name")]
        public string UserInitials { get; set; }

        public DateTime Date { get; set; }

        [ExcelColumn("Type")]
        public string ActivityCode { get; set; }

        public double Hours { get; set; }

        [ExcelColumn("Km")]
        public double Kilometers { get; set; }

        [ExcelColumn("Customer")]
        public string CustomerCode { get; set; }

        [ExcelColumn("Project")]
        public string ProjectCode { get; set; }

        [ExcelColumn("WBS code")]
        public string WBSCode { get; set; }

        public string Ticket { get; set; }
        public string Comment { get; set; }
    }
}