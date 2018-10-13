using KzsRest.Engine.Models;
using KzsRest.Engine.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        readonly IKzsParser kzsParser;
        public TeamsController(IKzsParser kzsParser)
        {
            this.kzsParser = kzsParser;
        }


        [HttpGet("{teamId:int}/season/{seasonId:int}")]
        public async Task<ActionResult<Team>> GetTeam(int teamId, int seasonId)
        {
            var result = await kzsParser.GetTeamAsync(teamId, seasonId, CancellationToken.None);
            return result;
        }
    }
}
