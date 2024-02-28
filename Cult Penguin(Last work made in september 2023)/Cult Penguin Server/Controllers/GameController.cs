using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin_Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> _logger;
        public GameController(ILogger<GameController> logger)
        {
            _logger = logger;
        }

        /*[HttpGet(Name = "Get/{username}")]
        public ActionResult<Account> GetAccount(string name)
        {
            Account user = LoginHandler.Instance.GetAccountByUsername(name);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound();
        }*/

        [HttpGet(Name = "get")]
        public ActionResult GetUpdateTimeStamp() {
            return Ok(ServerGameWorld.Instance.latestUpdateTimeStamp);
        }
    }
}
