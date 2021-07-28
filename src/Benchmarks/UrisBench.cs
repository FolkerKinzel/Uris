﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class UrisBench
    {
        private readonly StringBuilder _builder = new(16);
        private const string TEST = "test";

        //[Benchmark]
        //public StringBuilder AppendStackallock()
        //{
        //    return _builder.Append(stackalloc char[] { 't', 'e', 's', 't' }).Clear();
        //}

        //[Benchmark]
        //public StringBuilder AppendString() => _builder.Append("test").Clear();

        [Benchmark]
        public bool StartsWithString1()
            => TEST.AsSpan().StartsWith("test", StringComparison.OrdinalIgnoreCase);
        
        [Benchmark]
        public bool StartsWithString2()
            => TEST.StartsWith("test", StringComparison.OrdinalIgnoreCase);


        [Benchmark]
        public bool StartsWithStackallock()
            => TEST.AsSpan().StartsWith(stackalloc char[] { 't', 'e', 's', 't' }, StringComparison.OrdinalIgnoreCase);

    }
}
