﻿using System;
using Viki.LoadRunner.Playground.Replay;

namespace Viki.LoadRunner.Playground
{
    class Program
    {

        static void Main()
        {
            //BlankScenario.Run();

            //AssertPipeline.Run();

            //TheoreticalSpeedDemo.Run();

            //BlankStressScenarioMemoryStream.Run();

            //ReplayDemo.Run();

            //BatchStrategyDemo.Run();

            //new BlankFromBase().Validate();

            //DemoSetup.Run();

            // Broken
            LimitConcurrencyAndTpsDemo.Run();

            // Warning! Hdd/sdd intensive usage with this one
            //BlankStressScenarioJsonStream.Run();

            Console.ReadKey();
        }
    }
}
