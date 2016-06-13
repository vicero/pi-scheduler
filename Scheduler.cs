using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiScheduler
{
    /// <summary>
    /// Scheduler implementation
    /// </summary>
    internal sealed class Scheduler
    {
        /// <summary/>
        /// <param name="schedulableTasks">Tasks that will be scheduled and rescheduled.</param>
        /// <param name="granularityInMilliseconds">Defaults to 1000 ms if null.</param>
        public Scheduler(
            IReadOnlyList<PiTask> schedulableTasks,
            int? granularityInMilliseconds)
        {
            _schedulableTasks = schedulableTasks;
            _granularityInMilliseconds = granularityInMilliseconds ?? 1000;
            for (int i = _schedulableTasks.Count - 1; i >= 0; i--)
            {
                _taskQueue.Enqueue(_schedulableTasks[i]);
            }
        }

        /// <summary>
        /// Main loop entry point
        /// </summary>
        public void Run()
        {
            while (true)
            {
                var task = _taskQueue.Dequeue();

                if (task != null)
                {
                    RunTask(task);
                }
                else
                {
                    Task.Delay(_granularityInMilliseconds).Wait();
                }
            }
        }

        private void RunTask(PiTask task)
        {
            task.Execute();
            _taskQueue.Enqueue(task);
        }

        private readonly PiTaskQueue _taskQueue = new PiTaskQueue();
        private readonly IReadOnlyList<PiTask> _schedulableTasks;
        private readonly int _granularityInMilliseconds;
    }
}