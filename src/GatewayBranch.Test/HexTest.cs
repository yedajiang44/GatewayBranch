using System.Diagnostics;
using System.Linq;
using GatewayBranch.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace GatewayBranch.Test
{
    public class HexTest
    {
        private readonly byte[] data = Enumerable.Range(0, 1000).Select(x => (byte)x).ToArray();
        private readonly ITestOutputHelper outputHelper;

        public HexTest(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact(DisplayName = "linq转换16进制")]
        public void Test1()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Enumerable.Range(0, 10000).ToList().ForEach(_ => string.Join(" ", data.Select(x => x.ToString("16").PadLeft(2, '0'))).ToUpperInvariant());
            stopwatch.Stop();
            outputHelper.WriteLine($"耗时：{stopwatch.ElapsedMilliseconds} 毫秒");
        }

        [Fact(DisplayName = "编码表转换16进制")]
        public void Test2()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Enumerable.Range(0, 10000).ToList().ForEach(_ =>
            {
                var s = data.ToHexString();
            });
            stopwatch.Stop();
            outputHelper.WriteLine($"耗时：{stopwatch.ElapsedMilliseconds} 毫秒");
        }
    }
}
