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
using Docm.Business.Guest;
using Docm.Business.Guest.Imp;
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
using Swashbuckle.AspNetCore.Filters;

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

        //此方法由运行时调用。使用此方法将服务添加到容器。
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //添加资源文件地址
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            //添加api控制器服务
            services.AddControllers();
            #region Swagger说明文档配置
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });
            #endregion
            //添加jwt 验证服务 验证token 不用进到具体方法 可以进行验证 提高效率 但是无法跟进调试，打印bug问题
            services.Configure<TokenManagement>(Configuration.GetSection("tokenConfig"));
            var token = Configuration.GetSection("tokenConfig").Get<TokenManagement>();
            //jwt验证 1：(假设)属于第三方的数据验证
            //2:内部环境建议用对称加密，外部环境使用RSA加密（非对称加密，私有key与公有key加密，共有key暴露出去）
            services.AddJwtServices(token);
            //Authorization 请求通过之后验证
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admins", policy => policy
                    .RequireRole("Admin")
                    .RequireUserName("Arthur")
                    .RequireClaim("password"));
            });
            //添加域服务
            services.AddCors(options => options.AddPolicy(cors, builder => { 
                builder.WithOrigins(Configuration["Cores"].Split(','))//限制网站 为"*" 不限制
                .AllowAnyMethod()//允许所有类型的Method
                .AllowAnyHeader();//允许所有类型的Header
            }));
            
            services.AddSingleton(ConnectionMultiplexer.Connect(Configuration["Logging:RedisString"]));
            //redis引用
            //var redis = ConnectionMultiplexer.Connect(Configuration["Logging:RedisString"]);
            //redis.GetDatabase().SetAdd("lirui","123123");
            //添加cache缓存服务
            services.AddMemoryCache();
            //计时器
            services.AddSingleton<Stopwatch>();
            //epcore 添加数据库
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
            //sql数据库连接工厂
            services.AddSingleton<DbProviderFactory>(SqlClientFactory.Instance);
            services.AddScoped<IGuest, Guest>();
            services.AddScoped<GetJwtToken>();
        }

        //此方法由运行时调用。使用此方法配置HTTP请求管道。
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //Development判断是否测试(开发者)环境 env对象里面可以看到一系列当前环境配置 或者设置环境变量
            if (env.IsDevelopment())
            {
                //此中间件会详细列出报错详细信息，一般测试环境(开发者)下开启
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //生产环境错误处理
                app.UseExceptionHandler();
                //严格要求https请求
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
            //use 是更底层配置，自由化管道配置
            //app.Use(next =>
            //{
            //    logger.LogInformation("Use start....");
            //    //下面是中间件代码
            //    return async httpcontext =>
            //    {
            //        //请求的第一个路由路径是否包含first
            //        logger.LogInformation("async....");
            //        if (httpcontext.Request.Path.StartsWithSegments("/first"))
            //        {
            //            await httpcontext.Response.WriteAsync("/first");
            //            logger.LogInformation("first....");
            //        }
            //        else
            //        {
            //            //沿着管道继续向下执行
            //            logger.LogInformation("next(httpcontext)....");
            //            await next(httpcontext);
            //        }
            //    };
            //});
            //启动默认页面 并阻止下面管道执行
            //app.UseWelcomePage(new WelcomePageOptions()
            //{
            //    Path="/welcome"
            //});
            //添加http转https的中间件
            app.UseHttpsRedirection();
            //提供静态文件和启用默认文件映射
            //app.UseFileServer();
            //启用默认文件映射
            app.UseDefaultFiles();
            //提供静态文件
            app.UseStaticFiles();
            #region api说明文档配置
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
            #endregion
            //添加路由中间件 添加了这个必须添加UseEndpoints进行结尾，在两个中间可以添加其他的中间件
            app.UseRouting();
            //跨域
            app.UseCors(cors);
            //身份验证
            app.UseAuthentication();
            //添加请求授权信息中间件
            app.UseAuthorization();
            app.UseMiddleware(typeof(FinallyMiddeware));
            app.UseMiddleware(typeof(ExceptionMiddleware));
            app.UseMiddleware(typeof(TicketMiddleWare));
            //管道终结点
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
