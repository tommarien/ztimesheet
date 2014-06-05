using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices.Monitor;

namespace Timesheet.Tests.Monitor
{
    [TestFixture]
    public class When_monitoring_for_file_changes
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

            _monitor = new ConsolidatingFileSystemMonitor(TestHelper.WatchRoot, "*.*");
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
        public void it_notifies_that_file_did_change_after_grace_period_passes()
        {
            AppendToFile("file1", "Test");

            Thread.Sleep(_monitor.GracePeriod*2);

            _changedFiles.ToArray().ShouldBe(new[] {Path.Combine(TestHelper.WatchRoot, "file1")});
        }

        [Test]
        public void it_notifies_that_file_did_change_once_if_changed_twice_within_grace_period()
        {
            AppendToFile("file1", "Test");
            
            Thread.Sleep(2); // Make sure thread has time to do the work
            
            AppendToFile("file1", "Test line 2");

            Thread.Sleep(_monitor.GracePeriod*2);

            _changedFiles.Count.ShouldBe(1);
        }

        [Test]
        public void it_notifies_that_multiple_files_did_change_after_grace_period_passes()
        {
            AppendToFile("file1", "Test");
            AppendToFile("file2", "Test");

            Thread.Sleep(_monitor.GracePeriod*2);

            _changedFiles.ToArray().ShouldBe(new[] {Path.Combine(TestHelper.WatchRoot, "file1"), Path.Combine(TestHelper.WatchRoot, "file2")});
        }

        [Test]
        public void it_ignores_new_files_that_were_added_and_removed_within_grace_period()
        {
            AppendToFile("file1", "Test");
            Thread.Sleep(2);
            
            File.Delete(Path.Combine(TestHelper.WatchRoot, "file1"));
            Thread.Sleep(_monitor.GracePeriod * 2);

            _changedFiles.ShouldBeEmpty();
        }

        private void AppendToFile(string fileName, string line)
        {
            using (var stream = new StreamWriter(Path.Combine(TestHelper.WatchRoot, fileName), true))
            {
                stream.WriteLine(line);
            }
        }
    }
}