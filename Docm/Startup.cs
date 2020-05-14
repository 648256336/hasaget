using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MicroCommon.Middleware;
using MicroCommon.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace Docm
{
    public class Startup
    {
        
        private readonly string cors = "cors";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        //�˷���������ʱ���á�ʹ�ô˷�����������ӵ�������
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //�����Դ�ļ���ַ
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            //���api����������
            services.AddControllers();
            #region api˵���ĵ�����
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            #endregion
            //���jwt ��֤����
            services.Configure<TokenManagement>(Configuration.GetSection("tokenConfig"));
            var token = Configuration.GetSection("tokenConfig").Get<TokenManagement>();
            services.AddJwtServices(token);
            //��������
            services.AddCors(options => options.AddPolicy(cors, builder => { 
                builder.WithOrigins(Configuration["Cores"].Split(','))//������վ Ϊ"*" ������
                .AllowAnyMethod()//�����������͵�Method
                .AllowAnyHeader();//�����������͵�Header
            }));
            
            services.AddSingleton(ConnectionMultiplexer.Connect(Configuration["Logging:RedisString"]));
            //redis����
            //var redis = ConnectionMultiplexer.Connect(Configuration["Logging:RedisString"]);
            //redis.GetDatabase().SetAdd("lirui","123123");
            //���cache�������
            services.AddMemoryCache();
            //��ʱ��
            services.AddSingleton<Stopwatch>();
            //epcore ������ݿ�
           // services.AddDbContext<DataContext>(options =>
           //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            #region Localization
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("zh"),
                    new CultureInfo("ko"),
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider> {
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });
            #endregion
            //sql���ݿ����ӹ���
            services.AddSingleton<DbProviderFactory>(SqlClientFactory.Instance);
        }

        //�˷���������ʱ���á�ʹ�ô˷�������HTTP����ܵ���
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //Development�ж��Ƿ����(������)���� env����������Կ���һϵ�е�ǰ�������� �������û�������
            if (env.IsDevelopment())
            {
                //���м������ϸ�г�������ϸ��Ϣ��һ����Ի���(������)�¿���
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //��������������
                app.UseExceptionHandler();
                //�ϸ�Ҫ��https����
                app.UseHsts();
            }
            #region Localization
            var supportedCultures = new[]
            {
                    new CultureInfo("en"),
                    new CultureInfo("zh"),
                    new CultureInfo("ko"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>{
                       new AcceptLanguageHeaderRequestCultureProvider()
                }
            });
            #endregion
            //app.Use(next =>
            //{
            //    Console.WriteLine("Use one....");
            //    return async httpcontext =>
            //    {
            //        Console.WriteLine("async  one....");
            //        await next.Invoke(httpcontext);
            //        Console.WriteLine("async  one end....");
            //    };
            //});
            //app.Use(next =>
            //{
            //    Console.WriteLine("Use two....");
            //    return async httpcontext =>
            //    {
            //        Console.WriteLine("async  two....");
            //        await next.Invoke(httpcontext);
            //        Console.WriteLine("async  two end....");
            //    };
            //});
            //app.Use(next =>
            //{
            //    Console.WriteLine("Use three....");
            //    return async httpcontext =>
            //    {
            //        Console.WriteLine("async  three....");
            //        await next.Invoke(httpcontext);
            //        Console.WriteLine("async  three end....");
            //    };
            //});
            //use �Ǹ��ײ����ã����ɻ��ܵ�����
            //app.Use(next =>
            //{
            //    logger.LogInformation("Use start....");
            //    //�������м������
            //    return async httpcontext =>
            //    {
            //        //����ĵ�һ��·��·���Ƿ����first
            //        logger.LogInformation("async....");
            //        if (httpcontext.Request.Path.StartsWithSegments("/first"))
            //        {
            //            await httpcontext.Response.WriteAsync("/first");
            //            logger.LogInformation("first....");
            //        }
            //        else
            //        {
            //            //���Źܵ���������ִ��
            //            logger.LogInformation("next(httpcontext)....");
            //            await next(httpcontext);
            //        }
            //    };
            //});
            //����Ĭ��ҳ�� ����ֹ����ܵ�ִ��
            //app.UseWelcomePage(new WelcomePageOptions()
            //{
            //    Path="/welcome"
            //});
            //���httpתhttps���м��
            app.UseHttpsRedirection();
            //�ṩ��̬�ļ�������Ĭ���ļ�ӳ��
            //app.UseFileServer();
            //����Ĭ���ļ�ӳ��
            app.UseDefaultFiles();
            //�ṩ��̬�ļ�
            app.UseStaticFiles();
            #region api˵���ĵ�����
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            #endregion
            //���·���м�� ���������������UseEndpoints���н�β���������м��������������м��
            app.UseRouting();
            //����
            app.UseCors(cors);
            //�����֤
            app.UseAuthentication();
            //���������Ȩ��Ϣ�м��
            app.UseAuthorization();
            app.UseMiddleware(typeof(FinallyMiddeware));
            app.UseMiddleware(typeof(ExceptionMiddleware));
            app.UseMiddleware(typeof(TicketMiddleWare));
            //�ܵ��ս��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
