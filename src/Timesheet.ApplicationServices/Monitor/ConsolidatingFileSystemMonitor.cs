using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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

            GracePeriod = 75;
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
                pendingChanges.ForEach(path => _whenFileChanged(path));
            }
        }

        private void ProcessPendingEvents()
        {
            lock (_syncRoot)
            {
                DateTime now = DateTime.Now;

                var changesThatHavePassedGracePeriod = _pendingEvents
                    .Where(x => now.Subtract(x.Value).TotalMilliseconds >= GracePeriod)
                    .Select(x => x.Key)
                    .ToList();

                changesThatHavePassedGracePeriod.ForEach(path => _pendingEvents.Remove(path));

                if (_pendingEvents.Count == 0) StopTimer();

                changesThatHavePassedGracePeriod.ForEach(path => _whenFileChanged(path));
            }
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