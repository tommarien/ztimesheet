using System;
using Timesheet.ApplicationServices.Monitor;

namespace Timesheet.ApplicationServices
{
    public class FileMonitorService
    {
        private readonly ConsolidatingFileSystemMonitor _consolidatingFileSystemMonitor;

        public FileMonitorService(ConsolidatingFileSystemMonitor fileSystemMonitor)
        {
            _consolidatingFileSystemMonitor = fileSystemMonitor;
            _consolidatingFileSystemMonitor.WhenFileChanged(Console.WriteLine);
        }

        public void Start()
        {
            _consolidatingFileSystemMonitor.Start();
        }

        public void Stop()
        {
            _consolidatingFileSystemMonitor.Stop();
        }
    }
}