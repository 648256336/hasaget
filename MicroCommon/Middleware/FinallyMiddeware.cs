using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MicroCommon.Middleware
{
    public class FinallyMiddeware
    {
        private readonly RequestDelegate _next;
        private readonly Stopwatch _stopWatch;
        public FinallyMiddeware(RequestDelegate next, Stopwatch stopWatch)
        {
            _next = next;
            _stopWatch = stopWatch;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                _stopWatch.Start();
                await _next(context);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                //日志输出运行时间
                _stopWatch.Stop();
                Console.WriteLine(_stopWatch.Elapsed.TotalMilliseconds);
            }
        }
    }
}
