﻿using System;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Pool.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Core.Scheduler.Interfaces;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Speed
{
    public class LimitWorkingThreads : ISpeedStrategy
    {
        /// <summary>
        /// Count of worker-threads to allow executing iterations
        /// </summary>
        public int ThreadLimit;

        /// <summary>
        /// How much time to wait before thread will try to enqueue for iteration again
        /// </summary>
        public TimeSpan DelayInterval = TimeSpan.FromMilliseconds(100);

        private IThreadPoolStats _pool;
        private int _localIdlerCount = 0;
        private int _lastKnownCreatedThreadCount = 0;

        public LimitWorkingThreads(int threadLimit)
        {
            ThreadLimit = threadLimit;
        }

        public void Setup(ITestState state)
        {
            _pool = state.ThreadPool;
        }

        public void Next(IIterationId id, ISchedule scheduler)
        {
            bool thisThreadWasWorking = scheduler.Action == ScheduleAction.Execute;

            if (thisThreadWasWorking)
            {
                scheduler.Idle();
                Interlocked.Increment(ref _localIdlerCount);
            }
            else
            {
                int localIdlerCount = Interlocked.Decrement(ref _localIdlerCount);
                int estimatedworkingThreads = _pool.InitializedThreadCount - Math.Min(_pool.IdleThreadCount, localIdlerCount);

                if (estimatedworkingThreads <= ThreadLimit)
                {
                    scheduler.Execute();
                }
                else
                {
                    Interlocked.Increment(ref _localIdlerCount);
                    scheduler.Idle(DelayInterval);
                }
            }
        }

        public void HeartBeat(ITestState state)
        {
            // This part to be tested
            int currentCreatedThreadCount = _pool.CreatedThreadCount;
            if (currentCreatedThreadCount < _lastKnownCreatedThreadCount)
            {
                _localIdlerCount = 0;
                _lastKnownCreatedThreadCount = currentCreatedThreadCount;
            }
        }
    }
}