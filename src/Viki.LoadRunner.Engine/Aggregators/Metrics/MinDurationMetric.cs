﻿using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators.Metrics
{
    public class MinDurationMetric : MultiMetricBase<long>
    {
        private readonly string[] _ignoredCheckpoints;

        public MinDurationMetric(params string[] ignoredCheckpoints)
            : base(() => long.MaxValue)
        {
            if (ignoredCheckpoints == null)
                throw new ArgumentNullException(nameof(ignoredCheckpoints));

            _ignoredCheckpoints = ignoredCheckpoints.Union(new[] { Checkpoint.Names.Setup, Checkpoint.Names.Skip, Checkpoint.Names.TearDown }).ToArray();
        }

        protected override IMetric CreateNewMetric()
        {
            return new MinDurationMetric(_ignoredCheckpoints);
        }

        protected override void AddResult(IResult result)
        {
            ICheckpoint[] checkpoints = result.Checkpoints;
            for (int i = 0, j = checkpoints.Length - 1; i < j; i++)
            {
                ICheckpoint checkpoint = checkpoints[i];
                if (checkpoint.Error == null && _ignoredCheckpoints.All(name => name != checkpoint.Name))
                {
                    string key = "Min: " + checkpoint.Name;
                    TimeSpan momentDiff = checkpoint.TimePoint - checkpoints[i + 1].TimePoint;

                    if (_row[key] > momentDiff.TotalMilliseconds)
                        _row[key] = Convert.ToInt64(momentDiff.TotalMilliseconds);
                }
            }
        }
    }
}