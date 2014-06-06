using System.Threading;
using NUnit.Framework;
using Shouldly;

namespace Timesheet.Tests.Scheduling
{
    [TestFixture]
    public class TaskSchedulerTests
    {
        private ApplicationServices.TaskScheduler _scheduler;

        [SetUp]
        public void Setup()
        {
            _scheduler = new ApplicationServices.TaskScheduler();
        }

        [Test]
        public void it_should_execute_the_tasks_in_parallel()
        {
            bool _task1Completed = false;
            bool _task2Completed = false;

            _scheduler.Schedule(() => ActionActingAsTask(ref _task1Completed));
            _scheduler.Schedule(() => ActionActingAsTask(ref _task2Completed));

            _scheduler.AwaitAll();

            var completedFlags = new[] {_task1Completed, _task2Completed};

            completedFlags.ShouldAllBe(value => value == true);
        }

        [Test]
        public void it_should_remove_the_task_when_its_done()
        {
            bool _task1Completed = false;

            var taskId = _scheduler.Schedule(() => ActionActingAsTask(ref _task1Completed));

            _scheduler.Await(taskId);

            _scheduler.AmountOfPendingTasks.ShouldBe(0);
        }

        private void ActionActingAsTask(ref bool completed)
        {
            Thread.Sleep(100);

            completed = true;
        }
    }
}