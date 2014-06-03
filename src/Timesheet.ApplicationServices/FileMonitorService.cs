using System;
using System.IO;

namespace Timesheet.ApplicationServices
{
    public class FileMonitorService
    {
        private readonly FileSystemWatcher _fileSystemWatcher;

        public FileMonitorService(string path, string filter)
        {
            _fileSystemWatcher = new FileSystemWatcher(path, filter)
            {
                NotifyFilter = NotifyFilters.FileName
                               | NotifyFilters.DirectoryName
                               | NotifyFilters.LastWrite
                               | NotifyFilters.LastAccess
                               | NotifyFilters.Attributes
            };

            _fileSystemWatcher.Changed += (sender, e) => Console.WriteLine(e.FullPath);
            _fileSystemWatcher.Deleted += (sender, e) => Console.WriteLine(e.FullPath + " Deleted");
        }

        public void Start()
        {
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
        }
    }
}