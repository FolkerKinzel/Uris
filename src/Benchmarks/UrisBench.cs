using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using FolkerKinzel.Uris;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class UrisBench
    {
        //private readonly StringBuilder _builder = new(16);
        //private const string TEST = "test";

        private readonly DataUrl _dataUrlText1;
        private readonly DataUrl _dataUrlText2;

        public UrisBench()
        {
            const string data = "Märchenbücher";
            const string isoEncoding = "iso-8859-1";

#if NETSTANDARD2_0_OR_GREATER || NET5_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            string s = $"data:;charset={isoEncoding};base64,{Convert.ToBase64String(Encoding.GetEncoding(isoEncoding).GetBytes(data))}";

            _dataUrlText1 = DataUrl.Parse(s);
            _dataUrlText2 = DataUrl.Parse(DataUrl.FromText(data));
        }


        //[Benchmark]
        //public StringBuilder AppendStackallock()
        //{
        //    return _builder.Append(stackalloc char[] { 't', 'e', 's', 't' }).Clear();
        //}

        //[Benchmark]
        //public StringBuilder AppendString() => _builder.Append("test").Clear();

        //[Benchmark]
        //public bool StartsWithString1()
        //    => TEST.AsSpan().StartsWith("test", StringComparison.OrdinalIgnoreCase);

        //[Benchmark]
        //public bool StartsWithString2()
        //    => TEST.StartsWith("test", StringComparison.OrdinalIgnoreCase);


        //[Benchmark]
        //public bool StartsWithStackallock()
        //    => TEST.AsSpan().StartsWith(stackalloc char[] { 't', 'e', 's', 't' }, StringComparison.OrdinalIgnoreCase);


        [Benchmark]
        public bool EqualsBench()
        {
            return _dataUrlText1.Equals(_dataUrlText2);
        }
    }
}
