using System;
using System.Diagnostics;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Scenario
{
    /// <summary>
    /// Checkpoint holds information of timestamp and error of of current iteration.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class Checkpoint : ICheckpoint
    {
        #region Consts

        public static class Names
        {
            public static string Setup = "ITERATION_SETUP";
            public static string Skip = "ITERATION_SKIP";
            public static string IterationStart = "ITERATION_START";
            public static string IterationEnd = "ITERATION_END"; 
            public static string TearDown = "ITERATION_TEARDOWN";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the checkpoint (Checkpoint has const's of system checkpoints)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Timestamp of when this checkpoint was made durring iteration
        /// </summary>
        public TimeSpan TimePoint { get; set; }

        /// <summary>
        /// Error logged during the iteration or null.
        /// </summary>
        public Exception Error { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create checkpoint
        /// </summary>
        /// <param name="name">name of the checkpoint</param>
        /// <param name="timePoint">Timestamp of the checkpoint</param>
        public Checkpoint(string name, TimeSpan timePoint)
        {
            Name = name;
            TimePoint = timePoint;
        }

        #endregion
    }
}