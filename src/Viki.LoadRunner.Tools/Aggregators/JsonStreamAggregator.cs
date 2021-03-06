﻿using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Tools.Extensions;

namespace Viki.LoadRunner.Tools.Aggregators
{
    public class JsonStreamAggregator : FileStreamAggregatorBase
    {
        #region Constructors

        public JsonStreamAggregator(string jsonOutputfile) : base(jsonOutputfile)
        {
        }

        public JsonStreamAggregator(Func<string> dynamicJsonOutputFile) : base(dynamicJsonOutputFile)
        {
        }

        #endregion

        #region FileStreamAggregatorBase Write()

        public override void Write(string filePath, IEnumerable<IResult> stream)
        {
            stream.SerializeToJson(filePath);
        }

        #endregion

        #region Replay functions

        public static void Replay(string jsonResultsFile, params IAggregator[] targetAggregators)
        {
            Replay<object>(jsonResultsFile, targetAggregators);
        }

        public static void Replay<TUserData>(string jsonResultsFile, params IAggregator[] targetAggregators)
        {
            IEnumerable<IResult> resultsStream = Load<TUserData>(jsonResultsFile);

            StreamAggregator.Replay(resultsStream, targetAggregators);
        }

        public static IEnumerable<ReplayResult<TUserData>> Load<TUserData>(string jsonResultsFile)
        {
            return JsonStream.DeserializeFromJson<ReplayResult<TUserData>>(jsonResultsFile);
        }

        #endregion
    }
}