using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PuppeteerSharpTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet, Route("GetTest"), AllowAnonymous]
        public async Task<IActionResult> GetTestAsync()
        {
            try
            {
                for (int i = 1; i <= 100; i++)
                {
                    PuppeteerSharpUtil.ScreenshotAsync(url: "https://www.baidu.com", savePath: "E://");
                }
                return Content("成功");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
