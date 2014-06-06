using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheet.ApplicationServices
{
    public class TaskScheduler
    {
        private readonly ConcurrentDictionary<Guid, Task> _tasks = new ConcurrentDictionary<Guid, Task>();

        public Guid Schedule(Action action)
        {
            var taskId = Guid.NewGuid();

            var task = Task.Factory
                .StartNew(action)
                .ContinueWith((t) => AfterExecution(t, taskId));

            _tasks.TryAdd(taskId, task);

            return taskId;
        }

        public void Await(Guid taskId)
        {
            Task task;

            if (_tasks.TryGetValue(taskId, out task))
            {
                task.Wait();
            }
        }

        public void AwaitAll()
        {
            Task.WaitAll(_tasks.Values.ToArray());
        }

        private void AfterExecution(Task ancestor, Guid key)
        {
            Task removedTask;
            _tasks.TryRemove(key, out removedTask);

            if (ancestor.IsFaulted)
                throw ancestor.Exception;
        }

        public int AmountOfPendingTasks
        {
            get { return _tasks.Count; }
        }
    }
}