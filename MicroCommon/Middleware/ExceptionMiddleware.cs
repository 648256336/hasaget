using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroCommon.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                //返回友好的提示
                var response = context.Response;
                //response.StatusCode = (int)HttpStatusCode.OK;
                //可以增加其他继承Exception 接口判断状态
                //if (e is Exception ex)
                //{
                //    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //}
                //输出错误信息
                //var msg = rst.ToJson();
                await response.WriteAsync(e.Message).ConfigureAwait(false);
            }
        }
    }
}
