﻿using System;
using Viki.LoadRunner.Engine.Analytics.Interfaces;

namespace Viki.LoadRunner.Engine.Analytics.Metrics
{
    public class FuncMetric<T, TValue> : IMetric<T>
    {
        private readonly Func<T, TValue, TValue> _metricFunc;
        private readonly TValue _initialValue;

        private TValue _value;

        private readonly OutputFormatter _outputFormatter;

        public FuncMetric(string keyName, TValue initialValue, Func<T, TValue, TValue> metricFunc)
         : this(keyName, initialValue, metricFunc, DefaultOutputFormatter)
        {
        }

        public FuncMetric(string keyName, TValue initialValue, Func<T, TValue, TValue> metricFunc, OutputFormatter formatter)
        {
            ColumnNames = new[] { keyName };  
            _value = initialValue;
            _initialValue = initialValue;
            _metricFunc = metricFunc;
            _outputFormatter = formatter;
        }

        public IMetric<T> CreateNew()
        {
            return new FuncMetric<T, TValue>(ColumnNames[0], _initialValue, _metricFunc, _outputFormatter);
        }

        public void Add(T data)
        {
            _value = _metricFunc(data, _value);
        }

        public string[] ColumnNames { get; }
        public object[] Values => new [] { _outputFormatter(_value) };

        private static object DefaultOutputFormatter(TValue value) => value;

        public delegate object OutputFormatter(TValue value);
    }
}