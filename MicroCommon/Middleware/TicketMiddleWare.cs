using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicroCommon.Middleware
{
    public class TicketMiddleWare
    {
        private readonly RequestDelegate _next;
        public readonly IMemoryCache _cache;
        public TicketMiddleWare(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }
        public async Task Invoke(HttpContext context)
        {
            await _next(context);
        }
    }
}
