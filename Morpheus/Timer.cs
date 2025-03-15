
namespace Morpheus
{
    /// <summary>
    /// A simple timer that runs every interval.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// Action to run on trigger.
        /// </summary>
        public Action? Action = null!;

        /// <summary>
        /// How many times this timer was triggered.
        /// </summary>
        public ulong TriggeredCount { get; internal set; }

        /// <summary>
        /// Interval, in seconds, to trigger this timer.
        /// </summary>
        public float Interval = 1f;

        /// <summary>
        /// If true, this timer will be paused.
        /// </summary>
        public bool Paused;

        /// <summary>
        /// How much time passed, in seconds, since last trigger.
        /// </summary>
        public float ElapsedTime { get; internal set; }

        /// <summary>
        /// Create the timer.
        /// </summary>
        /// <param name="action">Action to call when triggered.</param>
        /// <param name="interval">Timer's interval.</param>
        internal Timer(Action? action, float interval = 1f)
        {
            Action = action;
        }

        /// <summary>
        /// Reset timer elapsed time.
        /// </summary>
        public void Reset()
        {
            ElapsedTime = 0f;
        }
    }
}
