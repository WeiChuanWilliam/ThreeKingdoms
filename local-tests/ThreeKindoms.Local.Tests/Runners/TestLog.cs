using System;
using System.Text;

namespace ThreeKindoms.Local.Tests.Runners
{
    static class TestLog
    {
        public static void Line(StringBuilder log, string line)
        {
            log.AppendLine(line);
            Console.WriteLine(line);
        }
    }
}
