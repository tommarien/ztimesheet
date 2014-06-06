using System;
using Timesheet.ApplicationServices.Excel;
using Timesheet.ApplicationServices.Monitor;
using Timesheet.ApplicationServices.Processor;

namespace Timesheet.ApplicationServices
{
    public class FileMonitorService
    {
        private readonly TaskScheduler _scheduler;
        private readonly TimeEntryRowReader _entryRowReader;
        private readonly TimeEntryProcessor _processor;
        private readonly ConsolidatingFileSystemMonitor _consolidatingFileSystemMonitor;

        public FileMonitorService(ConsolidatingFileSystemMonitor fileSystemMonitor, TaskScheduler scheduler, TimeEntryRowReader entryRowReader, TimeEntryProcessor processor)
        {
            _scheduler = scheduler;
            _entryRowReader = entryRowReader;
            _processor = processor;
            _consolidatingFileSystemMonitor = fileSystemMonitor;
            _consolidatingFileSystemMonitor.WhenFileChanged(fileName => _scheduler.Schedule(() => OnChangedFile(fileName)));
        }

        public void Start()
        {
            _consolidatingFileSystemMonitor.Start();
        }

        public void Stop()
        {
            _consolidatingFileSystemMonitor.Stop();

            _scheduler.AwaitAll();
        }

        private void OnChangedFile(string fileName)
        {
            var timeEntries = _entryRowReader.Read(fileName, new TimeEntryRowFilter {SkipEmptyLines = true});

            _processor.Process(timeEntries);
        }
    }
}