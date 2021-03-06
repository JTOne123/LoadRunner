﻿using System;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    public class IncrementalLimitWorkingThreads : LimitWorkingThreads, ISpeedStrategy
    {
        private readonly int _initialWorkingThreadCount;
        private readonly TimeSpan _increaseTimePeriod;
        private readonly int _increaseBatchSize;

        /// <summary>
        /// Limits working thread count, and incrementally changes the limit in a way defined by parameters
        /// </summary>
        /// <param name="initialWorkingThreadCount">Initial amount of concurrent-working-threads</param>
        /// <param name="increaseTimePeriod">After what duration concurrent-working-thread limit will be increased by [increaseBatchSize]</param>
        /// <param name="increaseBatchSize">How much concurrent-working-thread count will be increased each time the [increaseTimePeriod] hits</param>
        public IncrementalLimitWorkingThreads(int initialWorkingThreadCount, TimeSpan increaseTimePeriod, int increaseBatchSize)
            : base(initialWorkingThreadCount)
        {
            _initialWorkingThreadCount = initialWorkingThreadCount;
            _increaseTimePeriod = increaseTimePeriod;
            _increaseBatchSize = increaseBatchSize;

            _currentAllowedCount = _initialWorkingThreadCount;
        }

        public new void HeartBeat(ITestState state)
        {
            HeartBeatInner(state.Timer.Value);
            base.HeartBeat(state);
        }

        private int _currentAllowedCount;

        private void HeartBeatInner(TimeSpan timerValue)
        {
            int count = GetAllowedMaxWorkingThreadCount(timerValue);
            if (count != _currentAllowedCount)
            {
                _currentAllowedCount = count;

                ThreadLimit = _currentAllowedCount;
            }
        }

        private int GetAllowedMaxWorkingThreadCount(TimeSpan testExecutionTime)
        {
            return (((int)(testExecutionTime.TotalMilliseconds / _increaseTimePeriod.TotalMilliseconds)) * _increaseBatchSize) + _initialWorkingThreadCount;
        }
    }
}