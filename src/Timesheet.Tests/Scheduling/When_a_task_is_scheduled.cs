using System;
using System.Threading;
using NUnit.Framework;
using Shouldly;
using Timesheet.ApplicationServices;

namespace Timesheet.Tests.Scheduling
{
    [TestFixture]
    public class When_a_task_is_scheduled
    {
        private TaskScheduler _scheduler;
        private bool _completed = false;

        [SetUp]
        public void Setup()
        {
            _completed = false;
            _scheduler = new TaskScheduler();
        }

        [Test]
        public void it_returns_a_task_id()
        {
            var taskId = _scheduler.Schedule(() => ImaginaryTask(100));
            taskId.ShouldNotBe(Guid.Empty);
        }

        [Test]
        public void it_increments_the_pending_task()
        {
            _scheduler.Schedule(() => ImaginaryTask(100));

            _scheduler.AmountOfPendingTasks.ShouldBe(1);
        }

        [Test]
        public void it_allows_waiting_for_task_completion()
        {
            var taskId = _scheduler.Schedule(() => ImaginaryTask(100));

            _scheduler.Await(taskId);

            _completed.ShouldBe(true);
        }

        [Test]
        public void it_removes_the_task_when_its_done()
        {
            _scheduler.Schedule(() => ImaginaryTask(100));

            Thread.Sleep(150);

            _scheduler.AmountOfPendingTasks.ShouldBe(0);
        }

        [Test]
        public void it_throws_the_exception_if_task_generated_an_error()
        {
            var taskId = _scheduler.Schedule(() => ImaginaryTask(100, true));

            Should.Throw<AggregateException>(() => _scheduler.Await(taskId));
        }

        private void ImaginaryTask(int sleepInMs, bool throwEx = false)
        {
            Thread.Sleep(sleepInMs);

            if (throwEx) throw new InvalidOperationException();

            _completed = true;
        }
    }
}