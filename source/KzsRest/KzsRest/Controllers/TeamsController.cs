using KzsRest.Engine;
using KzsRest.Engine.Keys;
using KzsRest.Engine.Services.Abstract;
using KzsRest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KzsRest.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        readonly IKzsParser kzsParser;
        readonly ICacheService cacheService;
        public TeamsController(IKzsParser kzsParser, ICacheService cacheService)
        {
            this.kzsParser = kzsParser;
            this.cacheService = cacheService;
        }

        [HttpGet("{teamId:int}/season/{seasonId:int}")]
        public async Task<ActionResult<Team>> GetTeam(int teamId, int seasonId)
        {
            try
            {
                var result = await cacheService.GetDataAsync(
                    new TeamKey(teamId, seasonId),
                    TimeSpan.FromMinutes(15),
                    (k, ct) => kzsParser.GetTeamAsync(k, ct), CancellationToken.None);
                return result;
            }
            catch (DomParsingException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
