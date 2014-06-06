using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Castle.Core.Logging;

namespace Timesheet.ApplicationServices.Monitor
{
    public class ConsolidatingFileSystemMonitor : IDisposable
    {
        private FileSystemWatcher _fileSystemWatcher;
        private Action<string> _whenFileChanged = e => { };
        private readonly Dictionary<string, DateTime> _pendingEvents = new Dictionary<string, DateTime>();
        private readonly object _syncRoot = new object();
        private readonly Timer _timer;
        private bool _timerStarted;
        private ILogger _logger = NullLogger.Instance;

        public ConsolidatingFileSystemMonitor(string path, string filter)
        {
            _timer = new Timer(OnTimerTriggered, null, Timeout.Infinite, Timeout.Infinite);

            _fileSystemWatcher = new FileSystemWatcher(path, filter)
            {
                NotifyFilter = NotifyFilters.FileName
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.LastWrite
                               | NotifyFilters.LastAccess
                               | NotifyFilters.Attributes
            };

            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Error += FileSystemWatcherOnError;

            GracePeriod = 1500;
        }

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        /// <summary>
        /// The amount of ms grace period for two changes on same file to be considered as one
        /// </summary>
        public int GracePeriod { get; set; }

        public string Path
        {
            get { return _fileSystemWatcher.Path; }
        }

        public void WhenFileChanged(Action<string> action)
        {
            if (action != null)
            {
                _whenFileChanged = action;
            }
        }

        public void Start()
        {
            if (_fileSystemWatcher.EnableRaisingEvents) return;

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            if (!_fileSystemWatcher.EnableRaisingEvents) return;

            _fileSystemWatcher.EnableRaisingEvents = false;

            if (_timerStarted)
            {
                StopTimer();
            }

            FlushPendingEvents();
        }

        private void FlushPendingEvents()
        {
            lock (_syncRoot)
            {
                var pendingChanges = _pendingEvents.Keys.ToList();

                pendingChanges.ForEach(path => _pendingEvents.Remove(path));
                pendingChanges.ForEach(NotifyChanged);
            }
        }

        private void NotifyChanged(string path)
        {
            Logger.Debug(() => string.Format("Notifying changed file '{0}'", path));

            _whenFileChanged(path);
        }

        private void ProcessPendingEvents()
        {
            List<string> changesThatHavePassedGracePeriod;

            lock (_syncRoot)
            {
                DateTime now = DateTime.Now;

                changesThatHavePassedGracePeriod = _pendingEvents
                    .Where(x => now.Subtract(x.Value).TotalMilliseconds >= GracePeriod)
                    .Select(x => x.Key)
                    .ToList();

                changesThatHavePassedGracePeriod.ForEach(path => _pendingEvents.Remove(path));

                if (_pendingEvents.Count == 0) StopTimer();
            }

            changesThatHavePassedGracePeriod.ForEach(NotifyChanged);
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }

            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.Dispose();
                _fileSystemWatcher = null;
            }
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            lock (_syncRoot)
            {
                _pendingEvents[e.FullPath] = DateTime.Now;

                if (!_timerStarted) StartTimer();
            }
        }

        private void FileSystemWatcherOnDeleted(object sender, FileSystemEventArgs e)
        {
            lock (_syncRoot)
            {
                _pendingEvents.Remove(e.FullPath);
            }
        }

        private void FileSystemWatcherOnError(object sender, ErrorEventArgs e)
        {
            Logger.Error("FileSystemWatcher caught an error", e.GetException());
        }

        private void StartTimer()
        {
            _timer.Change(100, 100);
            _timerStarted = true;
        }

        private void StopTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timerStarted = false;
        }

        private void OnTimerTriggered(object state)
        {
            ProcessPendingEvents();
        }
    }
}