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
        /// <param name="args">�ⲿִ�еĲ��� ���� �磺dotnet run 11111</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// netcore 3.1 ��ʱû���ṩvs����Դ��(����ȥgithub�������������鿴) ���Ի��� 2.2�汾 ���ڹ���-ѡ��-�ı��༭��-c#-�߼� ��֧�ֵ������������ 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>

            //����Ĭ�����ã��ṩ��δ�������Ҳ����������������������ã�ע�⣺ Ĭ���������ϵͳ������ ��ȡ��ͬjson���ã��������ⲿ��д���ý��и���
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //�����������
                    webBuilder.UseStartup<Startup>();
                });
    }
}
