using System;
using System.Collections.Generic;

namespace PiScheduler
{
    /// <summary>
    /// Maintains a sorted queue of <see cref="PiTask"/>s and ensures type safety
    /// </summary>
    internal class PiTaskQueue
    {
        /// <summary>
        /// Enqueues a task at the correct index based on <see cref="PiTask.NextOccurrence"/>.
        /// </summary>
        public void Enqueue(PiTask task)
        {
            var taskCount = _tasks.Count;

            for (int i = 0; i < taskCount; i++)
            {
                var t = (_tasks[i]);
                if (task.NextOccurrence <= t.NextOccurrence)
                {
                    _tasks.Insert(i, task);
                    return;
                }
            }

            _tasks.Add(task);
        }

        /// <summary>
        /// Dequeues a task at the head of the queue if it is time to run it.
        /// </summary>
        /// <returns>Next task to run if <see cref="NextOccurrence"/> &lt;= <see cref="DateTime.Now.Ticks"/>; otherwise null </returns>
        public PiTask Dequeue()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                var task = _tasks[i];
                if (IsTaskCategoryRunning(task))
                {
                    // skip over any tasks with a category matching the running task list
                    continue;
                }
                if (task.NextOccurrence <= DateTime.Now.Ticks)
                {
                    _tasks.RemoveAt(i);
                    _runningTasks.Add(task);
                    return task;
                }
            }

            ClearRunningTasks();

            return null;
        }

        private void ClearRunningTasks()
        {
            for (int i = _runningTasks.Count - 1; i >= 0; i--)
            {
                var task = _runningTasks[i];
                if (task.Complete <= DateTime.Now.Ticks)
                {
                    task.OnComplete();
                    _runningTasks.RemoveAt(i);
                }
            }
        }

        private bool IsTaskCategoryRunning(PiTask task)
        {
            for (int i = 0; i < _runningTasks.Count; i++)
            {
                var t = _runningTasks[i];
                if (t.Category == task.Category)
                {
                    return true;
                }
            }
            return false;
        }
        
        private readonly List<PiTask> _runningTasks = new List<PiTask>();
        private readonly List<PiTask> _tasks = new List<PiTask>();
    }
}
