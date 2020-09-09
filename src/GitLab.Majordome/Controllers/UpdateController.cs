using System.Threading.Tasks;
using GitLab.Majordome.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace GitLab.Majordome.Controllers
{
    [Route("api/[controller]")]
    public class UpdateController : Controller
    {
        private readonly IBotUpdateHandler botUpdateHandler;

        public UpdateController(IBotUpdateHandler botUpdateHandler)
        {
            this.botUpdateHandler = botUpdateHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            await botUpdateHandler.EchoAsync(update);
            return Ok();
        }
    }
}
