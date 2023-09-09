using System.Diagnostics;
using System;
using UnityModManagerNet;

namespace CustomNpcPortraits
{
    internal static class Loggers
    {
        public static void Log(string str)
        {
            Main.Logger.Log(str);
        }

        public static void Log(object obj)
        {
            Main.Logger.Log(obj?.ToString() ?? "null");
        }

        public class ProcessLog : IDisposable
        {
            private Stopwatch stopWatch = new Stopwatch();
            private string message;
            public ProcessLog(string message)
            {
                this.message = message;
                stopWatch.Start();
            }

            public void Dispose()
            {
                stopWatch.Stop();
                Main.Logger.Log($"{message}: [{stopWatch?.Elapsed:ss\\.ff}]");
            }
        }
    }
}
