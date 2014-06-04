using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices.Monitor;

namespace Timesheet.Tests.Monitor
{
    [TestFixture]
    public class When_stopping_to_monitor_for_file_changes
    {
        private ConsolidatingFileSystemMonitor _monitor;
        private List<string> _changedFiles;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            TestHelper.ResetRoot();
        }

        [SetUp]
        public void Setup()
        {
            _changedFiles = new List<string>();

            _monitor = new ConsolidatingFileSystemMonitor(TestHelper.WatchRoot) {GracePeriod = 15000};
            _monitor.WhenFileChanged(_changedFiles.Add);
            _monitor.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _monitor.Stop();
            _monitor.Dispose();
        }

        [Test]
        public void it_notifies_that_file_did_change()
        {
            AppendToFile("file1", "Test");

            Thread.Sleep(10);

            _monitor.Stop();

            _changedFiles.ToArray().ShouldBe(new[] {Path.Combine(TestHelper.WatchRoot, "file1")});
        }

        private void AppendToFile(string fileName, string line)
        {
            using (var stream = new StreamWriter(Path.Combine(TestHelper.WatchRoot, fileName), true))
            {
                stream.WriteLine(line);
                stream.Flush();
            }
        }
    }
}