using System;
using System.Diagnostics;

namespace PiScheduler
{
    /// <summary>
    /// Task that will be scheduled and ran by the scheduler.
    /// </summary>
    [DebuggerDisplay("{Name} Next Occurrence { NextOccurrence }")]
    internal abstract class PiTask
    {
        public PiTask(int recurrence, string name, string category, int? taskLength = null)
        {
            //1ms minimum recurrence
            var minimumRecurrence = 1;
            if (recurrence <= minimumRecurrence)
            {
                throw new Exception("Recurrence must be greater than " + minimumRecurrence + " ms");
            }
            if (recurrence <= taskLength)
            {
                throw new Exception("recurrence must be greater than taskLength");
            }
            _category = category;
            _name = name;
            _recurrence = recurrence * System.TimeSpan.TicksPerMillisecond;
            _taskLength = taskLength.GetValueOrDefault(0) > 0 ? taskLength.Value * TimeSpan.TicksPerMillisecond : NoLength;
        }

        /// <summary>
        /// Called by <see cref="Scheduler"/>.
        /// </summary>
        /// <remarks>
        /// Updates <see cref="NextOccurrence"/> then performs <see cref="DoWork"/>
        /// </remarks>
        public void Execute()
        {
            ComputeCompleteAndNextOccurrence();
            DoWork();
        }

        /// <summary>
        /// Complete callback.
        /// </summary>
        public abstract void OnComplete();

        /// <summary>
        /// The work to complete.
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// Sets <see cref="Complete"/> and <see cref="NextOccurrence"/> to <see cref="DateTime.Now.Ticks"/> + <see cref="Recurrence"/>.
        /// </summary>
        private void ComputeCompleteAndNextOccurrence()
        {
            var currentTicks = DateTime.Now.Ticks;
            _complete = currentTicks + _taskLength;
            _nextOcurrence = currentTicks + _recurrence + _taskLength;
        }

        /// <summary>
        /// Category of the task.
        /// </summary>
        /// <remarks>
        /// Tasks in the same category will run exclusively.
        /// </remarks>
        public string Category { get { return _category; } }

        /// <summary>
        /// Complete time in ticks.
        /// </summary>
        public long Complete { get { return _complete; } }

        /// <summary>
        /// Name of the task.
        public string Name { get { return _name; } }

        /// <summary>
        /// Next Occurence in ticks.
        /// </summary>
        public long NextOccurrence { get { return _nextOcurrence; } }

        public long TaskLength { get { return _taskLength; } }

        public const long NoLength = -1;

        private string _category;
        private long _complete;
        private string _name;
        private long _nextOcurrence;
        private long _recurrence;
        private long _taskLength;
    }
}
