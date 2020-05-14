using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Docm
{
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">外部执行的参数 命令 如：dotnet run 11111</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// netcore 3.1 暂时没有提供vs在线源码(可以去github里面下载下来查看) 可以换成 2.2版本 ，在工具-选项-文本编辑器-c#-高级 将支持到导航反编译打开 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>

            //创建默认配置，提供如何处理请求，也可以在这里添加其他的配置，注意： 默认这里根据系统变量名 读取不同json配置，可以在外部重写配置进行覆盖
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //添加启动配置
                    webBuilder.UseStartup<Startup>();
                });
    }
}
