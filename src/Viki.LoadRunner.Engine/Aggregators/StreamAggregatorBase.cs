﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline;
using Viki.LoadRunner.Engine.Core.Collector.Pipeline.Extensions;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// StreamAggregator provides loadtest raw/masterdata (IResult) IEnumerable stream 
    /// </summary>
    public abstract class StreamAggregatorBase : IAggregator, IDisposable
    {
        #region Fields

        private BatchingPipe<IResult> _queue;
        private Task _writerTask;

        #endregion

        #region Abstract

        protected abstract void Process(IEnumerable<IResult> stream);

        #endregion

        #region IResultsAggregator

        public void Begin()
         {
            _queue = new BatchingPipe<IResult>();

             _writerTask = _queue.ToEnumerableAsync(Process);
        }

        public void Aggregate(IResult result)
        {
            AssertTask();

            _queue.Produce(result);
        }

        public void End()
        {
            if (_queue != null)
            { 
                _queue.ProducingCompleted();
                _queue = null;
                
                _writerTask.Wait();

                AssertTask();
            }
        }

        #endregion

        #region IDisposable

        ~StreamAggregatorBase()
        {
            Dispose();
        }

        public void Dispose()
        {
            ((IAggregator)this).End();
        }

        #endregion

        #region Misc

        private void AssertTask()
        {
            if (_writerTask.Exception != null)
                throw _writerTask.Exception;
        }

        #endregion
    }
    

}