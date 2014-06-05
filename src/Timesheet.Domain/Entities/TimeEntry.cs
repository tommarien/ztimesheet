using System;

namespace Timesheet.Domain.Entities
{
    public class TimeEntry
    {
        public virtual Guid Id { get; protected set; }
        public virtual string User { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string Activity { get; set; }
        public virtual double Hours { get; set; }
        public virtual double Kilometers { get; set; }
        public virtual string Customer { get; set; }
        public virtual string Project { get; set; }
        public virtual string WBSCode { get; set; }
        public virtual string Ticket { get; set; }
        public virtual string Comment { get; set; }
    }
}