using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Docms;
using Docm.Business.Guest.Imp;
using Microsoft.AspNetCore.Authorization;
using Docm.Data;

namespace Docm.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IStringLocalizer _localizer;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IGuest _guest;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IStringLocalizerFactory factory, IGuest guest)
        {
            _logger = logger;
            _localizer = factory.Create(typeof(SharedResource));
            _guest = guest;
        }

        [HttpGet("Get")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("LocalizerTest")]
        public async Task<IActionResult>  LocalizerTest()
        {
            EFData data = new EFData();
            data.Database.EnsureDeleted();
            data.Database.EnsureCreated();
            return Ok(_localizer["密码为空"]);
        }
        [HttpGet("TestActionResult")]
        //[Authorize(Roles = "Admin")]//权限
        [Authorize(Policy = "admins")]//策略
        public async Task<IActionResult> TestActionResult(int code)
        {
            var data = "12321";
            return StatusCode(code, data);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet("Logon")]
        public async Task<IActionResult> Logon(string account, string password)
        {
            var token= await _guest.Logon(account, password);
            if(token.HasValue())
            return StatusCode(200, token);
            else
                return StatusCode(400, "Jwt生成异常");
        }
    }
}
