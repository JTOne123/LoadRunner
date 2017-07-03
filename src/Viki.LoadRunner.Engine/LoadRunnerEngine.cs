﻿using System;
using System.Threading;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Executor.Threads;
using Viki.LoadRunner.Engine.Executor.Timer;
using Viki.LoadRunner.Engine.Parameters;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Speed;

namespace Viki.LoadRunner.Engine
{
    /// <summary>
    /// ILoadTestScenario executor
    /// </summary>
    public class LoadRunnerEngine
    {
        #region Fields

        private readonly ILoadRunnerSettings _settings;
        private readonly Type _iTestScenarioObjectType;

        private ILimitStrategy _limits;

        private readonly IResultsAggregator _aggregator;

        private Thread _rootThread;

        #region Run() globals

        private readonly ExecutionTimer _timer = new ExecutionTimer();
        private ThreadCoordinator _coordinator;

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Current duration of currently executing load test
        /// </summary>
        public TimeSpan TestDuration => _timer.Value;
        /// <summary>
        /// Start UTC time for currently executing load test
        /// </summary>
        public DateTime TestBeginTimeUtc => _timer.BeginTime;

        /// <summary>
        /// Indicates whether LoadTest is currently running
        /// </summary>
        public bool IsRunning => _timer.IsRunning;

        /// <summary>
        /// If execution failed due to unhandled exception, it will be set here.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes new executor instance
        /// </summary>
        /// <param name="settings">LoadTest parameters</param>
        /// <param name="iTestScenarioObjectType">ILoadTestScenario to be executed object type</param>
        /// <param name="resultsAggregators">Aggregators to use when aggregating results from all iterations</param>
        public LoadRunnerEngine(ILoadRunnerSettings settings, Type iTestScenarioObjectType, params IResultsAggregator[] resultsAggregators)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));
            if (iTestScenarioObjectType == null)
                throw new ArgumentNullException(nameof(iTestScenarioObjectType));

            _settings = settings;

            _iTestScenarioObjectType = iTestScenarioObjectType;

            _aggregator = new AsyncResultsAggregator(resultsAggregators);
        }

        /// <summary>
        /// Initializes new executor instance
        /// </summary>
        /// <typeparam name="TTestScenario">ILoadTestScenario to be executed object type</typeparam>
        /// <param name="parameters">LoadTest parameters</param>
        /// <param name="resultsAggregators">Aggregators to use when aggregating results from all iterations</param>
        /// <returns></returns>
        public static LoadRunnerEngine Create<TTestScenario>(LoadRunnerParameters parameters, params IResultsAggregator[] resultsAggregators) 
            where TTestScenario : ILoadTestScenario
        {
            return new LoadRunnerEngine(parameters, typeof(TTestScenario), resultsAggregators);
        }

        #endregion

        #region Async/Run()

        /// <summary>
        /// Start LoadTest execution.
        /// This is a blocking call and will finish only, once the test is over and all results are aggregated by IResultsAggregator's
        /// </summary>
        public void Run()
        {
            RunAsync();
            Wait();
        }

        /// <summary>
        /// Executes load test in seperate thread (non-blocking call)
        /// </summary>
        public void RunAsync()
        {
            if (IsLoadEngineRunning)
                throw new InvalidOperationException("Another instance is already running");

            _rootThread = new Thread(RunInner);
            _rootThread.Start();
        }

        /// <summary>
        /// Cancels Async test execution.
        /// Stops exeucion safely with time-out handling.
        /// Aggregated data up to this point won't be lost. 
        /// </summary>
        public void CancelAsync(bool blocking = true)
        {
            // TODO: Refactor a bit to allow clean way to stop this
            // This inits the stop.
            _limits = new LimitStrategy
            {
                MaxIterationsCount = 0,
                FinishTimeout = _limits.FinishTimeout,
            };

            if (blocking)
                Wait();
        }

        /// <summary>
        /// Waits, till execution is finished.
        /// </summary>
        /// <param name="timeOut">timeout time period to wait before returning</param>
        /// <param name="abortOnTimeOut">if execution won't finish within desired timeout, should it be terminated prematurely?</param>
        /// <returns>true - if test execution is stopped (either before timeout or aborted due to [abortOnTimeOut])</returns>
        public bool Wait(TimeSpan timeOut, bool abortOnTimeOut = false)
        {
            bool result = _rootThread.Join(timeOut);
            if (abortOnTimeOut == true && result == false)
            {
                _rootThread.Abort();
                _rootThread?.Join();
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Waits Infinitely until loadtest execution is finished
        /// </summary>
        public void Wait()
        {
            _rootThread?.Join();
        }

        private bool IsLoadEngineRunning => _rootThread?.IsAlive == true;

        #endregion

        #region RunInner() Stuff

        private void RunInner()
        {
            if (_coordinator != null)
                throw new InvalidOperationException("Async instance is already running");

            Exception = null;

            try
            {
                _limits = _settings.Limits;
                _coordinator = new ThreadCoordinator(new CoordinatorSettings
                {
                    InitialUserData = _settings.InitialUserData,
                    Scenario = _iTestScenarioObjectType,
                    Scheduler = new PrioritySpeedStrategy(_settings.SpeedPriority, _settings.Speed),
                    Timer = _timer,
                    Aggregator = _aggregator
                });

                CoordinatorContext context = _coordinator.Context;

                ISpeedStrategy scheduler = context.Scheduler;
                IThreadingStrategy threading = _settings.Threading;

                InitialThreadingSetup(_coordinator, threading);

                StartTest(context, _timer);

                while (!_limits.StopTest(context))
                {
                    _coordinator.AssertThreadErrors();

                    threading.Adjust(context, _coordinator);
                    scheduler.Adjust(context);

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
                throw;
            }
            finally
            {
                _coordinator?.StopAndDispose((int)_limits.FinishTimeout.TotalMilliseconds);

                _aggregator.End();
                _timer.Stop();

                ThreadCoordinator local = _coordinator;
                _coordinator = null;
                local?.AssertThreadErrors();
            }
        }

        private static void StartTest(CoordinatorContext context, ExecutionTimer timer)
        {
            context.Aggregator.Begin();
            timer.Start(); // This line also releases Worker-Threads from wait.
        }

        private static void InitialThreadingSetup(ThreadCoordinator coordinator, IThreadingStrategy threading)
        {
            threading.Setup(coordinator.Context, coordinator);

            while (coordinator.CreatedThreadCount != coordinator.InitializedThreadCount)
            {
                Thread.Sleep(100);
                coordinator.AssertThreadErrors();
            }
        }

        #endregion
    }
}