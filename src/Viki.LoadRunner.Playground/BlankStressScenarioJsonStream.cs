﻿using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Tools.Aggregators;

namespace Viki.LoadRunner.Playground
{
    public class BlankStressScenarioJsonStream 
    {
        // HDD/SSD intensive, beware
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(2)))
                .Add(new CountMetric())
                .Add(new TransactionsPerSecMetric());

            HistogramAggregator aggregagorOriginal = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(2)))
                .Add(new CountMetric())
                .Add(new TransactionsPerSecMetric());

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario<BlankScenario>()
                .SetThreading(new FixedThreadCount(3))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(5)))
                .SetAggregator(new JsonStreamAggregator("Raw.json"), aggregagorOriginal);

            strategy.Build().Run();

            JsonStreamAggregator.Replay("Raw.json", aggregator);

            Console.WriteLine(JsonConvert.SerializeObject(aggregagorOriginal.BuildResultsObjects(), Formatting.Indented));
            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
        }
    }
}
