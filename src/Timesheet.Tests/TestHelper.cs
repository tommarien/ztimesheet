using System.IO;

namespace Timesheet.Tests
{
    public static class TestHelper
    {
        public static string WatchRoot { get; private set; }

        static TestHelper()
        {
            WatchRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Files", "WatchPath"));
        }

        public static void ResetRoot()
        {
            if (!Directory.Exists(WatchRoot))
                Directory.CreateDirectory(WatchRoot);
            else
            {
                foreach (var file in Directory.EnumerateFiles(WatchRoot))
                {
                    File.Delete(file);
                }
            }
        }
    }
}