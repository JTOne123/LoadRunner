﻿using System;
using System.Collections.Generic;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Aggregates;
using Viki.LoadRunner.Engine.Aggregators.Results;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Executor.Context;

namespace Viki.LoadRunner.Engine.Aggregators
{
    /// <summary>
    /// Calculates totals without doing any aggregations
    /// </summary>
    public class TotalsResultsAggregator : IResultsAggregator
    {
        #region Fields

        private readonly CheckpointOrderLearner _orderLearner = new CheckpointOrderLearner();
        private readonly TestContextResultAggregate _statsAggregator = new TestContextResultAggregate();

        #endregion

        #region IResultsAggregator

        void IResultsAggregator.TestContextResultReceived(TestContextResult result)
        {
            _orderLearner.Learn(result);
            _statsAggregator.AggregateResult(result);
        }

        void IResultsAggregator.Begin(DateTime testBeginTime)
        {
            _statsAggregator.Reset();
            _orderLearner.Reset();
        }

        void IResultsAggregator.End()
        {
        }

        #endregion

        #region GetResults()

        /// <summary>
        /// Get Build results object from aggregated data
        /// </summary>
        /// <returns>Aggregated results</returns>
        public ResultsContainer GetResults()
        {
            ResultsContainer result = null;
            if (_orderLearner.LearnedOrder.Count != 0)
            {
                ResultsMapper mapper = new ResultsMapper(_orderLearner);
                IEnumerable<ResultItemRow> resultRows = mapper.Map(_statsAggregator);
                result = new ResultsContainer(resultRows.ToList(), new ResultItemTotals(_statsAggregator));
            }
            return result;
        }

        #endregion
    }
}