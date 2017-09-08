﻿using System;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Context;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Scenario
{
    public interface IWorkerTask
    {
        void Init();
        void Wait();
        void Execute();
        void Cleanup();

        void Stop();
    }

    public class ScenarioWorkerTask : IWorkerTask
    {
        private readonly IScheduler _scheduler;
        private readonly ITestContext _context;


        private readonly IDataCollector _collector;

        private bool _stopping = false;

        public ScenarioWorkerTask(IScheduler scheduler, ITestContext context, IDataCollector collector)
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (collector == null)
                throw new ArgumentNullException(nameof(collector));

            _scheduler = scheduler;
            _context = context;
            _collector = collector;
        }

        public void Init()
        {
            _handler.Init();
        }

        public void Wait()
        {
            // Wait for ITimer to start.
            while (_scheduler.Timer.IsRunning == false && _stopping == false)
                Thread.Sleep(1);
        }

        public void Execute()
        {
            _handler.PrepareNext();

            _scheduler.WaitNext();

            if (!_stopping)
            {
                _handler.Execute();

                _collector.Collect(_context);
            }
        }

        public void Cleanup()
        {
            _handler.Cleanup();
        }

        public void Stop()
        {
            _stopping = true;
        }
    }
}