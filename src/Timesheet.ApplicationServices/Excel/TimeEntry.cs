using System;
using LinqToExcel.Attributes;

namespace Timesheet.ApplicationServices.Excel
{
    public class TimeEntry
    {
        [ExcelColumn("Name")]
        public string UserInitials { get; set; }

        public int Week { get; set; }
        public string Day { get; set; }
        public DateTime Date { get; set; }

        [ExcelColumn("Type")]
        public string ActivityCode { get; set; }

        public decimal Hours { get; set; }

        [ExcelColumn("Km")]
        public decimal Kilometers { get; set; }

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