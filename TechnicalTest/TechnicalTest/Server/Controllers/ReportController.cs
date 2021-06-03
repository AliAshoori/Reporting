using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechnicalTest.Server.Services;
using TechnicalTest.Shared;

namespace TechnicalTest.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportGenerator _reportGenerator;

        public ReportController(IReportGenerator reportGenerator)
        {
            _reportGenerator = reportGenerator.NotNull();
        }

        [HttpGet("Generate")]
        public async Task<IActionResult> GenerateAsync() => Accepted(await _reportGenerator.GenerateAsync());
    }
}
