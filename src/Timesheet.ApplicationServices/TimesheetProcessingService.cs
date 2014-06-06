using System;
using Castle.Core.Logging;
using Timesheet.ApplicationServices.Excel;
using Timesheet.ApplicationServices.Monitor;
using Timesheet.ApplicationServices.Processor;

namespace Timesheet.ApplicationServices
{
    public class TimesheetProcessingService
    {
        private readonly TaskScheduler _scheduler;
        private readonly TimeEntryRowReader _entryRowReader;
        private readonly TimeEntryProcessor _processor;
        private readonly ConsolidatingFileSystemMonitor _consolidatingFileSystemMonitor;
        private ILogger _logger = NullLogger.Instance;

        public TimesheetProcessingService(ConsolidatingFileSystemMonitor fileSystemMonitor, TaskScheduler scheduler, TimeEntryRowReader entryRowReader, TimeEntryProcessor processor)
        {
            _scheduler = scheduler;
            _entryRowReader = entryRowReader;
            _processor = processor;
            _consolidatingFileSystemMonitor = fileSystemMonitor;

            _consolidatingFileSystemMonitor.WhenFileChanged(fileName => _scheduler.Schedule(() => OnChangedFile(fileName)));
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public void Start()
        {
            _consolidatingFileSystemMonitor.Start();
        }

        public void Stop()
        {
            _consolidatingFileSystemMonitor.Stop();

            try
            {
                _scheduler.AwaitAll();
            }
            catch (Exception e)
            {
                Logger.FatalFormat(e, "Error occured while waiting for pending tasks");
            }
        }

        private void OnChangedFile(string fileName)
        {
            try
            {
                var timeEntries = _entryRowReader.Read(fileName, new TimeEntryRowFilter {SkipEmptyLines = true});

                _processor.Process(timeEntries);

                Logger.InfoFormat("Successfully processed '{0}'", fileName);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat(e, "Error occured while processing '{0}'", fileName);
            }
        }
    }
}