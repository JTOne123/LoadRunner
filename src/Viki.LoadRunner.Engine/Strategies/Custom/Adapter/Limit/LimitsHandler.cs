﻿using System;
using Viki.LoadRunner.Engine.Core.State.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies.Custom.Adapter.Limit
{
    public class LimitsHandler
    {
        private readonly ILimitStrategy[] _limits;

        public LimitsHandler(ILimitStrategy[] limits)
        {
            if (limits == null)
                throw new ArgumentNullException(nameof(limits));

            _limits = limits;
        }

        public bool StopTest(ITestState state)
        {
            for (int i = 0; i < _limits.Length; i++)
            {
                if (_limits[i].StopTest(state))
                    return true;
            }

            return false;
        }
    }
}