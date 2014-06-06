using System.Threading;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices;

namespace Timesheet.Tests.Scheduling
{
    [TestFixture]
    public class When_tasks_are_scheduled
    {
        private TaskScheduler _scheduler;
        private bool _task1Completed;
        private bool _task2Completed;

        [SetUp]
        public void Setup()
        {
            _scheduler = new TaskScheduler();

            _task1Completed = false;
            _task2Completed = false;
        }

        [Test]
        public void it_executes_the_tasks_in_parallel()
        {
            _scheduler.Schedule(ImaginaryTask);
            _scheduler.Schedule(ImaginaryTask2);

            _scheduler.AmountOfPendingTasks.ShouldBe(2);

            // A little bit more to allow threading magic
            Thread.Sleep(300);

            _task1Completed.ShouldBe(true);
            _task2Completed.ShouldBe(true);
        }

        [Test]
        public void it_allows_to_wait_for_all()
        {
            _scheduler.Schedule(ImaginaryTask);
            _scheduler.Schedule(ImaginaryTask2);

            _scheduler.AwaitAll();

            _task1Completed.ShouldBe(true);
            _task2Completed.ShouldBe(true);
        }

        private void ImaginaryTask()
        {
            Thread.Sleep(250);
            _task1Completed = true;
        }
        private void ImaginaryTask2()
        {
            Thread.Sleep(250);
            _task2Completed = true;
        }
    }
}